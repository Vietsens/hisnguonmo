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
using MPS.Processor.Mps000085.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000085
{
    class Mps000085Processor : AbstractProcessor
    {
        Mps000085PDO rdo;
        public Mps000085Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000085PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListMediMate", rdo.listAdo);
                objectTag.AddObjectData(store, "ListImpMestUser", rdo._ImpMestUserPrint);
                objectTag.AddObjectData(store, "ListRoleUserEnd", rdo.roleAdo);
                objectTag.AddObjectData(store, "ListImpMestBlood", rdo._ListAdo);
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
                decimal sumPrice = 0;
                decimal sumPriceNoVat = 0;
                List<string> listSupplier = new List<string>();   
                string supplierString = "";      
                if (rdo._ListImpMestBlood != null && rdo._ListImpMestBlood.Count > 0)
                {
                    rdo._ListAdo = (from r in rdo._ListImpMestBlood select new MPS.Processor.Mps000085.PDO.Mps000085PDO.BLOODADO(r)).ToList();
                    totalPrice = rdo._ListImpMestBlood.Sum(s => ((s.PRICE ?? 0) * (1 + s.VAT_RATIO ?? 0)));
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.TOTAL_PRICE, totalPrice));
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.TOTAL_PRICE_SEPARATE, Inventec.Common.Number.Convert.NumberToString(totalPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));
                    string totalPriceStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(totalPriceStr)));
                }
                if (this.rdo._ImpMestMedicines != null && this.rdo._ImpMestMedicines.Count > 0)
                {
                    // sắp xếp theo thứ tự tăng dần id 
                    this.rdo._ImpMestMedicines = this.rdo._ImpMestMedicines.OrderBy(o => o.ID).ToList();
                    foreach (var item in this.rdo._ImpMestMedicines)
                    {
                        ImpMestMedicineADO ado = new ImpMestMedicineADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ImpMestMedicineADO>(ado, item);
                        ado.BATCH_REGISTER_NUMBER = item.MEDICINE_REGISTER_NUMBER;
                        ado.BATCH_MANUFACTURER_CODE = item.MEDICINE_MANUFACTURER_CODE;
                        ado.BATCH_MANUFACTURER_NAME = item.MEDICINE_MANUFACTURER_NAME;
                        if (rdo._Medicines != null && rdo._Medicines.Count > 0)
                        {
                            ado.TDL_BID_GROUP_CODE = rdo._Medicines.FirstOrDefault(o => o.ID == item.MEDICINE_ID).TDL_BID_GROUP_CODE;
                            ado.TDL_BID_NUM_ORDER = rdo._Medicines.FirstOrDefault(o => o.ID == item.MEDICINE_ID).TDL_BID_NUM_ORDER;
                            ado.TDL_BID_NUMBER = rdo._Medicines.FirstOrDefault(o => o.ID == item.MEDICINE_ID).TDL_BID_NUMBER;
                            ado.TDL_BID_PACKAGE_CODE = rdo._Medicines.FirstOrDefault(o => o.ID == item.MEDICINE_ID).TDL_BID_PACKAGE_CODE;
                            ado.TDL_BID_YEAR = rdo._Medicines.FirstOrDefault(o => o.ID == item.MEDICINE_ID).TDL_BID_YEAR;
                            ado.VIR_IMP_PRICE = rdo._Medicines.FirstOrDefault(o => o.ID == item.MEDICINE_ID).VIR_IMP_PRICE;
                        }

                        if (rdo._ListMedicalContract != null && rdo._ListMedicalContract.Count > 0)
                        {
                            V_HIS_MEDICAL_CONTRACT MedicalContract = rdo._ListMedicalContract.FirstOrDefault(o => o.MEDICINE_ID == item.MEDICINE_ID);
                            if (MedicalContract != null)
                            {
                                ado.MEDICAL_CONTRACT_CODE = MedicalContract.MEDICAL_CONTRACT_CODE;
                                ado.MEDICAL_CONTRACT_NAME = MedicalContract.MEDICAL_CONTRACT_NAME;
                                ado.DOCUMENT_SUPPLIER_NAME = MedicalContract.DOCUMENT_SUPPLIER_NAME;
                                ado.VENTURE_AGREENING = MedicalContract.VENTURE_AGREENING;
                            }
                        }

                        this.rdo.listAdo.Add(new Mps000085ADO(ado));
                        if (!item.PRICE.HasValue)
                            continue;
                        sumPrice += item.AMOUNT * item.PRICE.Value * (1 + (item.IMP_VAT_RATIO));
                        sumPriceNoVat += item.AMOUNT * item.PRICE.Value;
                        listSupplier.Add(item.SUPPLIER_NAME);
                    }
                }
                if (this.rdo._ImpMestMaterials != null && this.rdo._ImpMestMaterials.Count > 0)
                {
                    // sắp xếp theo thứ tự tăng dần id
                    this.rdo._ImpMestMaterials = this.rdo._ImpMestMaterials.OrderBy(o => o.ID).ToList();
                    foreach (var item in this.rdo._ImpMestMaterials)
                    {
                        ImpMestMaterialADO ado = new ImpMestMaterialADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ImpMestMaterialADO>(ado, item);
                        ado.BATCH_REGISTER_NUMBER = item.MATERIAL_REGISTER_NUMBER;
                        ado.BATCH_MANUFACTURER_CODE = item.MATERIAL_MANUFACTURER_CODE;
                        ado.BATCH_MANUFACTURER_NAME = item.MATERIAL_MANUFACTURER_NAME;
                        if (rdo._Materials != null && rdo._Materials.Count > 0)
                        {
                            ado.TDL_BID_GROUP_CODE = rdo._Materials.FirstOrDefault(o => o.ID == item.MATERIAL_ID).TDL_BID_GROUP_CODE;
                            ado.TDL_BID_NUM_ORDER = rdo._Materials.FirstOrDefault(o => o.ID == item.MATERIAL_ID).TDL_BID_NUM_ORDER;
                            ado.TDL_BID_NUMBER = rdo._Materials.FirstOrDefault(o => o.ID == item.MATERIAL_ID).TDL_BID_NUMBER;
                            ado.TDL_BID_PACKAGE_CODE = rdo._Materials.FirstOrDefault(o => o.ID == item.MATERIAL_ID).TDL_BID_PACKAGE_CODE;
                            ado.TDL_BID_YEAR = rdo._Materials.FirstOrDefault(o => o.ID == item.MATERIAL_ID).TDL_BID_YEAR;
                            ado.VIR_IMP_PRICE = rdo._Materials.FirstOrDefault(o => o.ID == item.MATERIAL_ID).VIR_IMP_PRICE;
                        }

                        if (rdo._ListMedicalContract != null && rdo._ListMedicalContract.Count > 0)
                        {
                            V_HIS_MEDICAL_CONTRACT MedicalContract = rdo._ListMedicalContract.FirstOrDefault(o => o.MATERIAL_ID == item.MATERIAL_ID);
                            if (MedicalContract != null)
                            {
                                ado.MEDICAL_CONTRACT_CODE = MedicalContract.MEDICAL_CONTRACT_CODE;
                                ado.MEDICAL_CONTRACT_NAME = MedicalContract.MEDICAL_CONTRACT_NAME;
                                ado.DOCUMENT_SUPPLIER_NAME = MedicalContract.DOCUMENT_SUPPLIER_NAME;
                                ado.VENTURE_AGREENING = MedicalContract.VENTURE_AGREENING;
                            }
                        }

                        this.rdo.listAdo.Add(new Mps000085ADO(ado));
                        if (!item.PRICE.HasValue)
                            continue;
                        sumPrice += item.AMOUNT * item.PRICE.Value * (1 + (item.IMP_VAT_RATIO));
                        sumPriceNoVat += item.AMOUNT * item.PRICE.Value;
                        listSupplier.Add(item.SUPPLIER_NAME);
                    }
                }

                listSupplier = listSupplier.Distinct().ToList();
                if (listSupplier != null && listSupplier.Count > 0)
                {
                    foreach (var item in listSupplier)
                    {
                        supplierString += item + "; ";
                    }

                    if (!string.IsNullOrEmpty(supplierString))
                    {
                        SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.SUPPLIER_NAME_STR, supplierString));
                    }
                }

                if (this.rdo._ImpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.IMP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.rdo._ImpMest.IMP_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.DOCUMENT_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._ImpMest.DOCUMENT_DATE ?? 0)));
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.rdo._ImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._ImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.rdo._ImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.SUM_PRICE
, sumPrice));
                    string sumPriceSeparate = Inventec.Common.Number.Convert.NumberToString(sumPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.SUM_PRICE_SEPARATE
, sumPriceSeparate));
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.SUM_PRICE_NO_VAT
, sumPriceNoVat));
                    string sumPriceNoVatSeparate = Inventec.Common.Number.Convert.NumberToString(sumPriceNoVat, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.SUM_PRICE_NO_VAT_SEPARATE
, sumPriceNoVat));
                    string sumPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sumPrice));
                    string sumPriceStringNoVat = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sumPriceNoVat));
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceString)));
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.SUM_PRICE_TEXT_NO_VAT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceStringNoVat)));
                    decimal sumAfterDiscount = sumPrice * (1 - this.rdo._ImpMest.DISCOUNT_RATIO ?? 0);
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.SUM_PRICE_AFTER_DISCOUNT, sumAfterDiscount));
                    string sumAfterDiscountSeparate = Inventec.Common.Number.Convert.NumberToString(sumAfterDiscount, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.SUM_PRICE_AFTER_DISCOUNT_SEPARATE, sumAfterDiscountSeparate));
                    string sumAfterDiscountString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sumAfterDiscount));
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.SUM_PRICE_AFTER_DISCOUNT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumAfterDiscountString)));
                    string documentPriceSeparate = Inventec.Common.Number.Convert.NumberToString(this.rdo._ImpMest.DOCUMENT_PRICE ?? 0, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.DOCUMENT_PRICE, this.rdo._ImpMest.DOCUMENT_PRICE ?? 0));
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.DOCUMENT_PRICE_SEPARATE, documentPriceSeparate));
                    string documentPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.rdo._ImpMest.DOCUMENT_PRICE ?? 0));
                    SetSingleKey(new KeyValue(Mps000085ExtendSingleKey.DOCUMENT_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(documentPriceString)));
                    //AddObjectKeyIntoListkey(this.rdo._ImpMest, false);
                    AddObjectKeyIntoListkey(this.rdo._ImpMest, false);
                }

                if (rdo._bid != null)
                {
                    AddObjectKeyIntoListkey(this.rdo._bid, false);
                }

                if (rdo._supplier != null)
                {
                    AddObjectKeyIntoListkey(this.rdo._supplier, false);
                }

                if (rdo._ImpMestUserPrint == null)
                {
                    rdo._ImpMestUserPrint = new List<V_HIS_IMP_MEST_USER>();
                }

                if (rdo._ImpMestUserPrint != null && rdo._ImpMestUserPrint.Count > 0)
                {
                    RoleADO role = new RoleADO();

                    int count = 1;
                    foreach (var item in rdo._ImpMestUserPrint)
                    {
                        if (count > 10)
                            break;
                        System.Reflection.PropertyInfo piRole = typeof(RoleADO).GetProperty("Role" + count);
                        System.Reflection.PropertyInfo piUser = typeof(RoleADO).GetProperty("User" + count);
                        piRole.SetValue(role, item.EXECUTE_ROLE_NAME);
                        piUser.SetValue(role, item.USERNAME);
                        count++;
                    }
                    rdo.roleAdo.Add(role);
                }

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
                if (rdo != null && rdo._ImpMest != null)
                {
                    log = LogDataImpMest("", rdo._ImpMest.IMP_MEST_CODE, rdo._bid.BID_NAME);
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
                if (rdo != null && rdo._ImpMest != null)
                    result = String.Format("{0}_{1}_{2}_{3}", printTypeCode, rdo._ImpMest.IMP_MEST_CODE, rdo._ImpMest.MEDI_STOCK_CODE, rdo._ImpMest.SUPPLIER_CODE);
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
