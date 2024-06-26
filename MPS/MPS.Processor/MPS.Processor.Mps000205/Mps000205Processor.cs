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
using MPS.Processor.Mps000205.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000205
{
    class Mps000205Processor : AbstractProcessor
    {
        Mps000205PDO rdo;
        public Mps000205Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000205PDO)rdoBase;
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
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListMediMate", rdo.listAdo);
                objectTag.AddObjectData(store, "ListExpMestUser", rdo._ExpMestUserPrint);
                objectTag.AddObjectData(store, "ListRoleUserEnd", rdo.roleAdo);
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
                decimal sumPrice = 0;
                decimal sumPriceNoVat = 0;
                if (this.rdo._ExpMestMedicines != null && this.rdo._ExpMestMedicines.Count > 0)
                {
                    // sắp xếp theo thứ tự tăng dần id 
                    this.rdo._ExpMestMedicines = this.rdo._ExpMestMedicines.OrderBy(o => o.ID).ToList();

                    if (rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Approval || rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Export)
                    {
                        rdo._ExpMestMedicines = rdo._ExpMestMedicines.Where(o => o.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                    }
                    else if (rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Request || rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Reject || rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Draft)
                    {
                        rdo._ExpMestMedicines = rdo._ExpMestMedicines.Where(o => o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                    }

                    var listGroupMedicine = rdo._ExpMestMedicines.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.PACKAGE_NUMBER, g.SUPPLIER_ID });
                    var listMedicine = new List<V_HIS_EXP_MEST_MEDICINE>();

                    foreach (var item in listGroupMedicine)
                    {
                        var medicine = new V_HIS_EXP_MEST_MEDICINE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_MEDICINE>(medicine, item.First());
                        medicine.AMOUNT = item.Sum(s => s.AMOUNT);
                        listMedicine.Add(medicine);
                    }

                    foreach (var item in listMedicine)
                    {
                        this.rdo.listAdo.Add(new Mps000205ADO(item));
                        if (!item.PRICE.HasValue)
                            continue;
                        sumPrice += item.AMOUNT * item.PRICE.Value * (1 + (item.IMP_VAT_RATIO));
                        sumPriceNoVat += item.AMOUNT * item.PRICE.Value;
                    }
                }
                if (this.rdo._ExpMestMaterials != null && this.rdo._ExpMestMaterials.Count > 0)
                {
                    // sắp xếp theo thứ tự tăng dần id
                    this.rdo._ExpMestMaterials = this.rdo._ExpMestMaterials.OrderBy(o => o.ID).ToList();

                    if (rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Approval || rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Export)
                    {
                        rdo._ExpMestMaterials = rdo._ExpMestMaterials.Where(o => o.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MATERIAL.IN_EXECUTE__TRUE).ToList();
                    }
                    else if (rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Request || rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Reject || rdo._ExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Draft)
                    {
                        rdo._ExpMestMaterials = rdo._ExpMestMaterials.Where(o => o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MATERIAL.IN_REQUEST__TRUE).ToList();
                    }

                    var listGroupMaterial = rdo._ExpMestMaterials.GroupBy(g => new { g.MATERIAL_TYPE_ID, g.PACKAGE_NUMBER, g.SUPPLIER_ID });
                    var listMaterial = new List<V_HIS_EXP_MEST_MATERIAL>();

                    foreach (var item in listGroupMaterial)
                    {
                        var material = new V_HIS_EXP_MEST_MATERIAL();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_MATERIAL>(material, item.First());
                        material.AMOUNT = item.Sum(s => s.AMOUNT);
                        listMaterial.Add(material);
                    }

                    foreach (var item in listMaterial)
                    {
                        this.rdo.listAdo.Add(new Mps000205ADO(item));
                        if (!item.PRICE.HasValue)
                            continue;
                        sumPrice += item.AMOUNT * item.PRICE.Value * (1 + (item.IMP_VAT_RATIO));
                        sumPriceNoVat += item.AMOUNT * item.PRICE.Value;
                    }
                }

                if (this.rdo._ExpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000205ExtendSingleKey.EXP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.rdo._ExpMest.EXP_TIME ?? 0)));
                    //SetSingleKey(new KeyValue(Mps000205ExtendSingleKey.DOCUMENT_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._LostExpMest.DOCUMENT_DATE ?? 0)));
                    SetSingleKey(new KeyValue(Mps000205ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.rdo._ExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000205ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._ExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000205ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.rdo._ExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000205ExtendSingleKey.SUM_PRICE
, Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(sumPrice)));
                    SetSingleKey(new KeyValue(Mps000205ExtendSingleKey.SUM_PRICE_NO_VAT
, Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(sumPriceNoVat)));
                    string sumPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sumPrice));
                    string sumPriceStringNoVat = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sumPriceNoVat));
                    SetSingleKey(new KeyValue(Mps000205ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceString)));
                    SetSingleKey(new KeyValue(Mps000205ExtendSingleKey.SUM_PRICE_TEXT_NO_VAT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceStringNoVat)));
                    //decimal sumAfterDiscount = sumPrice - (this.rdo._LostExpMest.DISCOUNT ?? 0);
                    //SetSingleKey(new KeyValue(Mps000205ExtendSingleKey.SUM_PRICE_AFTER_DISCOUNT, sumAfterDiscount));
                    //string sumAfterDiscountString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sumAfterDiscount));
                    //SetSingleKey(new KeyValue(Mps000205ExtendSingleKey.SUM_PRICE_AFTER_DISCOUNT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumAfterDiscountString)));

                    //string documentPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.rdo._LostExpMest.DOCUMENT_PRICE ?? 0));
                    //SetSingleKey(new KeyValue(Mps000205ExtendSingleKey.DOCUMENT_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(documentPriceString)));

                    AddObjectKeyIntoListkey(this.rdo._ExpMest, false);
                    AddObjectKeyIntoListkey(this.rdo._ExpMest, false);
                }
                if (rdo._ExpMestUserPrint == null)
                {
                    rdo._ExpMestUserPrint = new List<V_HIS_EXP_MEST_USER>();
                }

                if (rdo._ExpMestUserPrint != null && rdo._ExpMestUserPrint.Count > 0)
                {
                    RoleADO role = new RoleADO();

                    int count = 1;
                    foreach (var item in rdo._ExpMestUserPrint)
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
