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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000347.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.BackendData;

namespace MPS.Processor.Mps000347
{
    class Mps000347Processor : AbstractProcessor
    {
        Mps000347PDO rdo;
        public Mps000347Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000347PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                List<Mps000347ADO> listAdoPrint = new List<Mps000347ADO>();
                if (rdo.listAdo != null && rdo.listAdo.Count > 0)
                {
                    if (false)
                    {
                        var dataGroup = rdo.listAdo.GroupBy(p => new { p.TYPE_ID, p.MEDI_MATE_TYPE_ID, p.PRICE, p.IMP_PRICE, p.IMP_VAT_RATIO }).ToList();
                        foreach (var itemGroup in dataGroup)
                        {
                            Mps000347ADO ado = new Mps000347ADO();
                            System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<Mps000347ADO>();
                            try
                            {
                                foreach (var item in pi)
                                {
                                    item.SetValue(ado, (item.GetValue(itemGroup.FirstOrDefault())));
                                }
                            }
                            catch (Exception ex)
                            {
                                LogSystem.Error(ex);
                            }
                            ado.TOTAL_AMOUNT = itemGroup.FirstOrDefault().TOTAL_AMOUNT;
                            ado.TOTAL_AMOUNT_IN_EXECUTE = itemGroup.Sum(p => p.TOTAL_AMOUNT_IN_EXECUTE);
                            ado.TOTAL_AMOUNT_IN_EXPORT = itemGroup.Sum(p => p.TOTAL_AMOUNT_IN_EXPORT);
                            ado.TOTAL_AMOUNT_IN_REQUEST = itemGroup.FirstOrDefault().TOTAL_AMOUNT_IN_REQUEST;
                            ado.MEDICINE_USE_FORM_NUM_ORDER = itemGroup.FirstOrDefault().MEDICINE_USE_FORM_NUM_ORDER;
                            ado.MEDICINE_TYPE_NAME = itemGroup.FirstOrDefault().MEDICINE_TYPE_NAME;
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


                    if (listAdoPrint != null && listAdoPrint.Count > 0)
                    {
                        long check = rdo.OderOption;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.OderOption), rdo.OderOption));
                        if (rdo.OderOption == 1)
                        {
                            listAdoPrint = listAdoPrint.OrderBy(p => p.TYPE_ID).ThenBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDI_MATE_TYPE_NAME).ToList();
                        }
                        else if (rdo.OderOption == 2)
                        {

                            listAdoPrint = listAdoPrint.OrderBy(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                        }
                        else if (rdo.OderOption == 3)
                        {
                            listAdoPrint = listAdoPrint.OrderByDescending(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                        }
                        else if (rdo.OderOption == 4)
                        {
                            listAdoPrint = listAdoPrint.OrderBy(p => p.SERVICE_UNIT_NAME).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listAdoPrint), listAdoPrint));
                }

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListMediMate1", listAdoPrint);
                objectTag.AddObjectData(store, "ListMediMate2", listAdoPrint);
                objectTag.AddObjectData(store, "ListMediMate3", listAdoPrint);
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
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        void ProcessSingleKey()
        {
            try
            {
                decimal totalPrice = 0;
                decimal totalPriceNoVat = 0;
                if (this.rdo._ChmsExpMest != null)
                {
                    var Req_Department = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == this.rdo._ChmsExpMest.REQ_DEPARTMENT_ID).ToList();
                    if (Req_Department != null && Req_Department.Count > 0)
                    {
                        this.rdo._ChmsExpMest.REQ_DEPARTMENT_NAME = Req_Department[0].DEPARTMENT_NAME;
                        this.rdo._ChmsExpMest.REQ_DEPARTMENT_CODE = Req_Department[0].DEPARTMENT_CODE;
                    }
                    SetSingleKey(new KeyValue(Mps000347ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000347ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000347ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    AddObjectKeyIntoListkey(this.rdo._ChmsExpMest, false);
                    if (BackendDataWorker.Get<V_HIS_MEDI_STOCK>() != null && BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Count > 0)
                    {
                        var dataImpMedi = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this.rdo._ChmsExpMest.IMP_MEDI_STOCK_ID);
                        if (dataImpMedi != null)
                        {
                            SetSingleKey(new KeyValue(Mps000347ExtendSingleKey.IMP_MEDI_STOCK_CODE, dataImpMedi.MEDI_STOCK_CODE));
                            SetSingleKey(new KeyValue(Mps000347ExtendSingleKey.IMP_MEDI_STOCK_NAME, dataImpMedi.MEDI_STOCK_NAME));
                        }
                        var data = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this.rdo._ChmsExpMest.MEDI_STOCK_ID);
                        if (data != null)
                        {
                            SetSingleKey(new KeyValue(Mps000347ExtendSingleKey.EXP_IS_CABINET, data.IS_CABINET));
                        }
                    }
                }

                #region Thuoc
                if (this.rdo._ExpMestMetyReqs != null && this.rdo._ExpMestMetyReqs.Count > 0)
                {
                    if (this.rdo._ExpMestMedicines != null && this.rdo._ExpMestMedicines.Count > 0)
                    {
                        var dataMediByMetyReqs = this.rdo._ExpMestMedicines.Where(p => this.rdo._ExpMestMetyReqs.Select(o => o.MEDICINE_TYPE_ID).ToList().Contains(p.MEDICINE_TYPE_ID)).ToList();
                        if (dataMediByMetyReqs != null && dataMediByMetyReqs.Count > 0)
                        {
                            var Group = dataMediByMetyReqs.GroupBy(g => new
                            {
                                g.MEDICINE_ID,
                                g.PACKAGE_NUMBER,
                                g.SUPPLIER_ID,
                                g.IMP_PRICE,
                                g.IMP_VAT_RATIO
                            }).ToList();

                            foreach (var mediGr in Group)
                            {
                                Mps000347ADO adoMediGr = new Mps000347ADO();
                                adoMediGr.TYPE_ID = 1;
                                adoMediGr.TOTAL_AMOUNT_IN_REQUEST = this.rdo._ExpMestMetyReqs.Where(p => p.MEDICINE_TYPE_ID
                                     == mediGr.First().MEDICINE_TYPE_ID).ToList().Sum(o => o.AMOUNT);
                                adoMediGr.TOTAL_AMOUNT = adoMediGr.TOTAL_AMOUNT_IN_REQUEST;
                                adoMediGr.MEDI_MATE_TYPE_CODE = mediGr.First().MEDICINE_TYPE_CODE;
                                adoMediGr.MEDI_MATE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(mediGr.First().MEDICINE_TYPE_ID.ToString() + adoMediGr.TYPE_ID.ToString());
                                adoMediGr.MEDI_MATE_TYPE_NAME = mediGr.First().MEDICINE_TYPE_NAME;
                                adoMediGr.ACTIVE_INGR_BHYT_NAME = mediGr.First().ACTIVE_INGR_BHYT_NAME;
                                adoMediGr.ACTIVE_INGR_BHYT_CODE = mediGr.First().ACTIVE_INGR_BHYT_CODE;
                                adoMediGr.REGISTER_NUMBER = mediGr.First().REGISTER_NUMBER;
                                adoMediGr.SERVICE_UNIT_CODE = mediGr.First().SERVICE_UNIT_CODE;
                                adoMediGr.SERVICE_UNIT_NAME = mediGr.First().SERVICE_UNIT_NAME;
                                adoMediGr.TOTAL_AMOUNT_IN_EXECUTE = mediGr.Sum(p => p.AMOUNT);
                                adoMediGr.PACKAGE_NUMBER = mediGr.First().PACKAGE_NUMBER;
                                adoMediGr.SUPPLIER_CODE = mediGr.First().SUPPLIER_CODE;
                                adoMediGr.SUPPLIER_NAME = mediGr.First().SUPPLIER_NAME;
                                adoMediGr.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(mediGr.First().EXPIRED_DATE ?? 0);
                                adoMediGr.PRICE = mediGr.First().PRICE;
                                adoMediGr.IMP_PRICE = mediGr.First().IMP_PRICE;
                                adoMediGr.IMP_VAT_RATIO = mediGr.First().IMP_VAT_RATIO;
                                adoMediGr.IMP_PRICE_RATIO = mediGr.First().IMP_PRICE + mediGr.First().IMP_PRICE * mediGr.First().IMP_VAT_RATIO;
                                adoMediGr.DESCRIPTION = mediGr.First().DESCRIPTION;
                                adoMediGr.MEDI_MATE_NUM_ORDER = mediGr.First().MEDICINE_NUM_ORDER ?? 0;
                                adoMediGr.NUM_ORDER = mediGr.First().NUM_ORDER;
                                adoMediGr.VIR_PRICE = mediGr.Sum(p => p.VIR_PRICE);
                                adoMediGr.MEDICINE_USE_FORM_NUM_ORDER = mediGr.First().MEDICINE_USE_FORM_NUM_ORDER;
                                adoMediGr.MEDICINE_TYPE_NAME = mediGr.First().MEDICINE_TYPE_NAME;

                                var _dataMedi = rdo._MedicineTypes.FirstOrDefault(p => p.ID == mediGr.First().MEDICINE_TYPE_ID);
                                if (_dataMedi != null)
                                {
                                    adoMediGr.NATIONAL_NAME = _dataMedi.NATIONAL_NAME;
                                    adoMediGr.PACKING_TYPE_NAME = _dataMedi.PACKING_TYPE_NAME;
                                    adoMediGr.MEDICINE_USE_FORM_NAME = _dataMedi.MEDICINE_USE_FORM_NAME;
                                    adoMediGr.CONCENTRA = _dataMedi.CONCENTRA;
                                }

                                var medicines = rdo._Medicines.Where(p => mediGr.Select(x => x.MEDICINE_ID).ToList().Contains(p.ID)).ToList();
                                adoMediGr.VIR_IMP_PRICE = medicines.Sum(p => p.VIR_IMP_PRICE);

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
                                    totalPriceNoVat += (item.AMOUNT * (item.IMP_PRICE)) - (item.DISCOUNT ?? 0);
                                }
                            }
                        }
                        else
                        {

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
                            Mps000347ADO ado = new Mps000347ADO();
                            ado.TYPE_ID = 1;
                            ado.DESCRIPTION = itemGr[0].DESCRIPTION;
                            var data = rdo._MedicineTypes.FirstOrDefault(p => p.ID == itemGr[0].MEDICINE_TYPE_ID);
                            if (data != null)
                            {
                                ado.MEDI_MATE_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                                ado.MEDI_MATE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(itemGr.First().MEDICINE_TYPE_ID.ToString() + ado.TYPE_ID.ToString());
                                ado.MEDI_MATE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                                ado.REGISTER_NUMBER = data.REGISTER_NUMBER;
                                ado.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                                ado.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                                ado.NATIONAL_NAME = data.NATIONAL_NAME;
                                ado.PACKING_TYPE_NAME = data.PACKING_TYPE_NAME;
                                ado.MEDICINE_USE_FORM_NAME = data.MEDICINE_USE_FORM_NAME;
                                ado.CONCENTRA = data.CONCENTRA;
                                ado.MEDICINE_TYPE_NAME = data.MEDICINE_TYPE_NAME;

                                if (rdo.useform != null && rdo.useform.Count() > 0)
                                    ado.MEDICINE_USE_FORM_NUM_ORDER = rdo.useform.FirstOrDefault(o => o.ID == data.MEDICINE_USE_FORM_ID).NUM_ORDER;
                            }

                            ado.TOTAL_AMOUNT_IN_REQUEST = itemGr.Sum(o => o.AMOUNT);
                            ado.TOTAL_AMOUNT = ado.TOTAL_AMOUNT_IN_REQUEST;
                            ado.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.TOTAL_AMOUNT_IN_REQUEST)));

                            rdo.listAdo.Add(ado);
                        }
                    }
                }
                #endregion

                #region VatTu
                if (this.rdo._ExpMestMatyReqs != null && this.rdo._ExpMestMatyReqs.Count > 0)
                {
                    if (this.rdo._ExpMestMaterials != null && this.rdo._ExpMestMaterials.Count > 0)
                    {
                        var dataMateByMatyReqs = this.rdo._ExpMestMaterials.Where(p => this.rdo._ExpMestMatyReqs.Select(o => o.MATERIAL_TYPE_ID).ToList().Contains(p.MATERIAL_TYPE_ID)).ToList();
                        if (dataMateByMatyReqs != null && dataMateByMatyReqs.Count > 0)
                        {
                            var Group = dataMateByMatyReqs.GroupBy(g => new
                            {
                                g.MATERIAL_TYPE_ID,
                                g.PACKAGE_NUMBER,
                                g.SUPPLIER_ID,
                                g.IMP_PRICE,
                                g.IMP_VAT_RATIO,
                                g.PRICE
                            }).ToList();

                            foreach (var mediGr in Group)
                            {
                                Mps000347ADO adoMediGr = new Mps000347ADO();
                                adoMediGr.TYPE_ID = 2;
                                adoMediGr.TOTAL_AMOUNT_IN_REQUEST = this.rdo._ExpMestMatyReqs.Where(p => p.MATERIAL_TYPE_ID
                                     == mediGr.First().MATERIAL_TYPE_ID).ToList().Sum(o => o.AMOUNT);
                                adoMediGr.TOTAL_AMOUNT = adoMediGr.TOTAL_AMOUNT_IN_REQUEST;
                                adoMediGr.MEDI_MATE_TYPE_CODE = mediGr.First().MATERIAL_TYPE_CODE;
                                adoMediGr.MEDI_MATE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(mediGr.First().MATERIAL_TYPE_ID.ToString() + adoMediGr.TYPE_ID.ToString());
                                adoMediGr.MEDI_MATE_TYPE_NAME = mediGr.First().MATERIAL_TYPE_NAME;
                                adoMediGr.SERVICE_UNIT_CODE = mediGr.First().SERVICE_UNIT_CODE;
                                adoMediGr.SERVICE_UNIT_NAME = mediGr.First().SERVICE_UNIT_NAME;
                                adoMediGr.TOTAL_AMOUNT_IN_EXECUTE = mediGr.Sum(p => p.AMOUNT);
                                adoMediGr.PACKAGE_NUMBER = mediGr.First().PACKAGE_NUMBER;
                                adoMediGr.SUPPLIER_CODE = mediGr.First().SUPPLIER_CODE;
                                adoMediGr.SUPPLIER_NAME = mediGr.First().SUPPLIER_NAME;
                                adoMediGr.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(mediGr.First().EXPIRED_DATE ?? 0);
                                adoMediGr.PRICE = mediGr.First().PRICE;
                                adoMediGr.IMP_PRICE = mediGr.First().IMP_PRICE;
                                adoMediGr.IMP_VAT_RATIO = mediGr.First().IMP_VAT_RATIO;
                                adoMediGr.IMP_PRICE_RATIO = mediGr.First().IMP_PRICE + mediGr.First().IMP_PRICE * mediGr.First().IMP_VAT_RATIO;
                                adoMediGr.DESCRIPTION = mediGr.First().DESCRIPTION;
                                adoMediGr.MEDI_MATE_NUM_ORDER = mediGr.First().MEDICINE_NUM_ORDER ?? 0;
                                adoMediGr.NUM_ORDER = mediGr.First().NUM_ORDER;
                                adoMediGr.VIR_PRICE = mediGr.Sum(p => p.VIR_PRICE);

                                var _dataMate = rdo._MaterialTypes.FirstOrDefault(p => p.ID == mediGr.First().MATERIAL_TYPE_ID);
                                if (_dataMate != null)
                                {
                                    adoMediGr.NATIONAL_NAME = _dataMate.NATIONAL_NAME;
                                    adoMediGr.PACKING_TYPE_NAME = _dataMate.PACKING_TYPE_NAME;
                                    adoMediGr.CONCENTRA = _dataMate.CONCENTRA;
                                }

                                var materials = rdo._Materials.Where(p => mediGr.Select(x => x.MATERIAL_ID).ToList().Contains(p.ID)).ToList();
                                adoMediGr.VIR_IMP_PRICE = materials.Sum(p => p.VIR_IMP_PRICE);
                                if (rdo._ChmsExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Export)
                                {
                                    adoMediGr.TOTAL_AMOUNT_IN_EXPORT = adoMediGr.TOTAL_AMOUNT_IN_EXECUTE;
                                }

                                adoMediGr.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(adoMediGr.TOTAL_AMOUNT_IN_REQUEST)));
                                adoMediGr.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(adoMediGr.TOTAL_AMOUNT_IN_EXECUTE)));
                                adoMediGr.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(adoMediGr.TOTAL_AMOUNT_IN_EXPORT)));

                                rdo.listAdo.Add(adoMediGr);

                                var listByGroup = mediGr.ToList<V_HIS_EXP_MEST_MATERIAL>();
                                foreach (var item in listByGroup)
                                {
                                    totalPrice += (item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                                    totalPriceNoVat += (item.AMOUNT * (item.IMP_PRICE)) - (item.DISCOUNT ?? 0);
                                }
                            }
                        }
                    }
                    else
                    {
                        var Groups = this.rdo._ExpMestMatyReqs.GroupBy(g => new
                        {
                            g.MATERIAL_TYPE_ID
                        }).Select(p => p.ToList()).ToList();
                        foreach (var itemGr in Groups)
                        {
                            Mps000347ADO ado = new Mps000347ADO();
                            ado.TYPE_ID = 2;
                            ado.DESCRIPTION = itemGr[0].DESCRIPTION;
                            var data = rdo._MaterialTypes.FirstOrDefault(p => p.ID == itemGr[0].MATERIAL_TYPE_ID);
                            if (data != null)
                            {
                                ado.MEDI_MATE_TYPE_CODE = data.MATERIAL_TYPE_CODE;
                                ado.MEDI_MATE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(itemGr.First().MATERIAL_TYPE_ID.ToString() + ado.TYPE_ID.ToString());
                                ado.MEDI_MATE_TYPE_NAME = data.MATERIAL_TYPE_NAME;
                                ado.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                                ado.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                                ado.NATIONAL_NAME = data.NATIONAL_NAME;
                                ado.PACKING_TYPE_NAME = data.PACKING_TYPE_NAME;
                                ado.CONCENTRA = data.CONCENTRA;
                            }

                            ado.TOTAL_AMOUNT_IN_REQUEST = itemGr.Sum(o => o.AMOUNT);
                            ado.TOTAL_AMOUNT = ado.TOTAL_AMOUNT_IN_REQUEST;
                            ado.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.TOTAL_AMOUNT_IN_REQUEST)));

                            rdo.listAdo.Add(ado);
                        }
                    }
                }
                #endregion

                SetSingleKey(new KeyValue(Mps000347ExtendSingleKey.KEY_NAMES, rdo._Tittle));
                SetSingleKey(new KeyValue(Mps000347ExtendSingleKey.SUM_TOTAL_PRICE, totalPrice));
                SetSingleKey(new KeyValue(Mps000347ExtendSingleKey.SUM_TOTAL_PRICE_NO_VAT, totalPriceNoVat));
                SetSingleKey(new KeyValue(Mps000347ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice)))));
                SetSingleKey(new KeyValue(Mps000347ExtendSingleKey.SUM_TOTAL_PRICE_NO_VAT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPriceNoVat)))));

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
                    log = LogDataImpMest("", rdo._ChmsExpMest.EXP_MEST_CODE, rdo._ChmsExpMest.REQ_DEPARTMENT_NAME);
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
                int countMedicine = 0;
                int countMaterial = 0;
                int countBlood = 0;
                string mediMateCode = "";

                if (rdo._ExpMestMedicines != null)
                {
                    countMedicine = rdo._ExpMestMedicines.Count;
                }

                if (rdo._ExpMestMaterials != null)
                {
                    countMaterial = rdo._ExpMestMaterials.Count;
                }

                if (rdo.listAdo != null && rdo.listAdo.Count > 0)
                    mediMateCode = rdo.listAdo.FirstOrDefault().MEDI_MATE_TYPE_CODE;

                if (rdo != null && rdo._ChmsExpMest != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}", printTypeCode, rdo._ChmsExpMest.EXP_MEST_CODE, rdo._ChmsExpMest.MEDI_STOCK_CODE, countMedicine, countMaterial, countBlood, mediMateCode);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("MPS.Processor.Mps000347 ProcessUniqueCodeData ", result));
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
