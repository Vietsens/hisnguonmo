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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000088.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000088
{
    class Mps000088Processor : AbstractProcessor
    {
        Mps000088PDO rdo;
        public Mps000088Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000088PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                //barCodeTag.ProcessData(store, dicImage);
                if (rdo.mps000088ByMediEndMate != null && rdo.mps000088ByMediEndMate.Count > 0)
                {
                    List<ServiceTypeADO> _ServiceTypeADOs = new List<ServiceTypeADO>();

                    var _Medicine = rdo.mps000088ByMediEndMate.FirstOrDefault(p => p.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC);
                    if (_Medicine != null)
                    {
                        ServiceTypeADO ado = new ServiceTypeADO();
                        ado.Service_Type_Id = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                        ado.TYPE_NAME = "Thuốc";
                        _ServiceTypeADOs.Add(ado);
                    }

                    var _Material = rdo.mps000088ByMediEndMate.FirstOrDefault(p => p.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                    if (_Material != null)
                    {
                        ServiceTypeADO ado = new ServiceTypeADO();
                        ado.Service_Type_Id = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                        ado.TYPE_NAME = "Vật tư";
                        _ServiceTypeADOs.Add(ado);
                    }

                    var _Blood = rdo.mps000088ByMediEndMate.FirstOrDefault(p => p.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU);
                    if (_Blood != null)
                    {
                        ServiceTypeADO ado = new ServiceTypeADO();
                        ado.Service_Type_Id = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU;
                        ado.TYPE_NAME = "Máu";
                        _ServiceTypeADOs.Add(ado);
                    }
                    objectTag.AddObjectData(store, "ServiceType", _ServiceTypeADOs);
                    objectTag.AddObjectData(store, "SereServ", rdo.mps000088ByMediEndMate);
                    objectTag.AddRelationship(store, "ServiceType", "SereServ", "Service_Type_Id", "Service_Type_Id");
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo.mps000088ADO != null && rdo.mps000088ADO.Count > 0)
                {
                    foreach (var item in rdo.mps000088ADO)
                    {
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day1, item.Day1));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day2, item.Day2));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day3, item.Day3));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day4, item.Day4));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day5, item.Day5));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day6, item.Day6));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day7, item.Day7));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day8, item.Day8));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day9, item.Day9));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day10, item.Day10));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day11, item.Day11));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day12, item.Day12));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day13, item.Day13));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day14, item.Day14));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day15, item.Day15));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day16, item.Day16));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day17, item.Day17));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day18, item.Day18));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day19, item.Day19));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day20, item.Day20));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day21, item.Day21));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day22, item.Day22));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day23, item.Day23));
                        SetSingleKey(new KeyValue(Mps000088ExtendSingleKey.Day24, item.Day24));

                    }
                }
                if (rdo.currentTreatment != null)
                {
                    SetSingleKey((new KeyValue(Mps000088ExtendSingleKey.IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.IN_TIME))));
                    if (rdo.currentTreatment.OUT_TIME != null)
                        SetSingleKey((new KeyValue(Mps000088ExtendSingleKey.OUT_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.OUT_TIME ?? 0))));
                    if (rdo.currentTreatment.CLINICAL_IN_TIME != null)
                        SetSingleKey((new KeyValue(Mps000088ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.CLINICAL_IN_TIME ?? 0))));
                    SetSingleKey((new KeyValue(Mps000088ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.currentTreatment.TDL_PATIENT_DOB))));
                }
                AddObjectKeyIntoListkey<SingleKeys>(rdo._SingleKeys);

                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.currentTreatment);

                if (rdo._vHisBedLog != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_BED_LOG>(rdo._vHisBedLog);
                }

                rdo.mps000088ByMediEndMate = GroupByService(rdo.mps000088ByMediEndMate);
                if (rdo.mps000088ByMediEndMate != null && rdo.mps000088ByMediEndMate.Count > 0 && rdo._SingleKeys.IsOderMedicine == 1)
                {
                    rdo.mps000088ByMediEndMate = rdo.mps000088ByMediEndMate.OrderBy(o => o.Service_Type_Id).ThenByDescending(o => o.MEDICINE_GROUP_NUM_ORDER).ThenByDescending(o => o.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(o => o.NUM_ORDER).ThenBy(o => o.MEDICINE_TYPE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<Mps000088ByMediEndMate> GroupByService(List<Mps000088ByMediEndMate> _Mps000225BySereServs)
        {
            List<MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate> result = new List<MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate>();
            if (_Mps000225BySereServs != null && _Mps000225BySereServs.Count > 0)
            {
                if (rdo._SingleKeys.IsTachHDSD)
                {
                    var rsGroup = _Mps000225BySereServs.GroupBy(p => new { p.SERVICE_ID, p.TUTORIAL, p.PRICE, p.Service_Type_Id, p.CONCENTRA, p.VAT_RATIO }).ToList();
                    foreach (var itemGroup in rsGroup)
                    {
                        MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate ado = new MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate();
                        Inventec.Common.Mapper.DataObjectMapper.Map<Mps000088ByMediEndMate>(ado, itemGroup.First());
                        ado.Service_Type_Id = itemGroup.FirstOrDefault().Service_Type_Id;
                        ado.AMOUNT = itemGroup.Sum(p => p.AMOUNT);
                        PropertyInfo[] ps = Inventec.Common.Repository.Properties.Get<MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate>();
                        foreach (var item in itemGroup)
                        {
                            for (int j = 0; j < 60; j++)
                            {
                                decimal itemValue = 0;
                                decimal itemValue2 = 0;
                                decimal itemValue3 = 0;
                                decimal itemValue4 = 0;
                                decimal itemValue5 = 0;

                                PropertyInfo propertyInfo = ps.FirstOrDefault(o => o.Name == string.Format("Day{0}", j + 1));
                                if (propertyInfo != null)
                                {
                                    itemValue = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo.GetValue(item) ?? "0").ToString());
                                    if (item != itemGroup.First())
                                        itemValue += Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo.GetValue(ado) ?? "0").ToString());

                                    if (itemValue > 0)
                                    {
                                        propertyInfo.SetValue(ado, itemValue.ToString());
                                    }
                                }
                                PropertyInfo propertyInfo2 = ps.FirstOrDefault(o => o.Name == string.Format("MORNING_Day{0}", j + 1));
                                if (propertyInfo2 != null)
                                {
                                    itemValue2 = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo2.GetValue(item) ?? "0").ToString());
                                    if (item != itemGroup.First())
                                        itemValue2 += Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo2.GetValue(ado) ?? "0").ToString());

                                    if (itemValue2 > 0)
                                    {
                                        propertyInfo2.SetValue(ado, itemValue2.ToString());
                                    }
                                }
                                PropertyInfo propertyInfo3 = ps.FirstOrDefault(o => o.Name == string.Format("NOON_Day{0}", j + 1));
                                if (propertyInfo3 != null)
                                {
                                    itemValue3 = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo3.GetValue(item) ?? "0").ToString());
                                    if (item != itemGroup.First())
                                        itemValue3 += Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo3.GetValue(ado) ?? "0").ToString());

                                    if (itemValue3 > 0)
                                    {
                                        propertyInfo3.SetValue(ado, itemValue3.ToString());
                                    }
                                }
                                PropertyInfo propertyInfo4 = ps.FirstOrDefault(o => o.Name == string.Format("AFTERNOON_Day{0}", j + 1));
                                if (propertyInfo4 != null)
                                {
                                    itemValue4 = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo4.GetValue(item) ?? "0").ToString());

                                    if (item != itemGroup.First())
                                        itemValue4 += Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo4.GetValue(ado) ?? "0").ToString());

                                    if (itemValue4 > 0)
                                    {
                                        propertyInfo4.SetValue(ado, itemValue4.ToString());
                                    }
                                }
                                PropertyInfo propertyInfo5 = ps.FirstOrDefault(o => o.Name == string.Format("EVENING_Day{0}", j + 1));
                                if (propertyInfo5 != null)
                                {
                                    itemValue5 = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo5.GetValue(item) ?? "0").ToString());

                                    if (item != itemGroup.First())
                                        itemValue5 += Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo5.GetValue(ado) ?? "0").ToString());

                                    if (itemValue5 > 0)
                                    {
                                        propertyInfo5.SetValue(ado, itemValue5.ToString());
                                    }
                                }
                                PropertyInfo propertyInfo6 = ps.FirstOrDefault(o => o.Name == string.Format("SUM_Day{0}", j + 1));
                                if (propertyInfo6 != null)
                                {
                                    decimal sum_Day = itemValue2 + itemValue3 + itemValue4 + itemValue5;
                                    if (sum_Day > 0)
                                    {
                                        propertyInfo6.SetValue(ado, sum_Day.ToString());
                                    }
                                }
                            }
                        }

                        result.Add(ado);
                    }
                }
                else
                {
                    var rsGroup = _Mps000225BySereServs.GroupBy(p => new { p.SERVICE_ID, p.PRICE, p.Service_Type_Id, p.CONCENTRA, p.VAT_RATIO }).ToList();
                    foreach (var itemGroup in rsGroup)
                    {
                        MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate ado = new MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate();
                        Inventec.Common.Mapper.DataObjectMapper.Map<Mps000088ByMediEndMate>(ado, itemGroup.First());
                        ado.Service_Type_Id = itemGroup.FirstOrDefault().Service_Type_Id;
                        ado.AMOUNT = itemGroup.Sum(p => p.AMOUNT);
                        PropertyInfo[] ps = Inventec.Common.Repository.Properties.Get<MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate>();
                        foreach (var item in itemGroup)
                        {
                            for (int j = 0; j < 60; j++)
                            {
                                decimal itemValue = 0;
                                decimal itemValue2 = 0;
                                decimal itemValue3 = 0;
                                decimal itemValue4 = 0;
                                decimal itemValue5 = 0;

                                PropertyInfo propertyInfo = ps.FirstOrDefault(o => o.Name == string.Format("Day{0}", j + 1));
                                if (propertyInfo != null)
                                {
                                    itemValue = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo.GetValue(item) ?? "0").ToString());
                                    if (item != itemGroup.First())
                                        itemValue += Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo.GetValue(ado) ?? "0").ToString());

                                    if (itemValue > 0)
                                    {
                                        propertyInfo.SetValue(ado, itemValue.ToString());
                                    }
                                }
                                PropertyInfo propertyInfo2 = ps.FirstOrDefault(o => o.Name == string.Format("MORNING_Day{0}", j + 1));
                                if (propertyInfo2 != null)
                                {
                                    itemValue2 = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo2.GetValue(item) ?? "0").ToString());
                                    if (item != itemGroup.First())
                                        itemValue2 += Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo2.GetValue(ado) ?? "0").ToString());

                                    if (itemValue2 > 0)
                                    {
                                        propertyInfo2.SetValue(ado, itemValue2.ToString());
                                    }
                                }
                                PropertyInfo propertyInfo3 = ps.FirstOrDefault(o => o.Name == string.Format("NOON_Day{0}", j + 1));
                                if (propertyInfo3 != null)
                                {
                                    itemValue3 = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo3.GetValue(item) ?? "0").ToString());
                                    if (item != itemGroup.First())
                                        itemValue3 += Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo3.GetValue(ado) ?? "0").ToString());

                                    if (itemValue3 > 0)
                                    {
                                        propertyInfo3.SetValue(ado, itemValue3.ToString());
                                    }
                                }
                                PropertyInfo propertyInfo4 = ps.FirstOrDefault(o => o.Name == string.Format("AFTERNOON_Day{0}", j + 1));
                                if (propertyInfo4 != null)
                                {
                                    itemValue4 = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo4.GetValue(item) ?? "0").ToString());

                                    if (item != itemGroup.First())
                                        itemValue4 += Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo4.GetValue(ado) ?? "0").ToString());

                                    if (itemValue4 > 0)
                                    {
                                        propertyInfo4.SetValue(ado, itemValue4.ToString());
                                    }
                                }
                                PropertyInfo propertyInfo5 = ps.FirstOrDefault(o => o.Name == string.Format("EVENING_Day{0}", j + 1));
                                if (propertyInfo5 != null)
                                {
                                    itemValue5 = Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo5.GetValue(item) ?? "0").ToString());

                                    if (item != itemGroup.First())
                                        itemValue5 += Inventec.Common.TypeConvert.Parse.ToDecimal((propertyInfo5.GetValue(ado) ?? "0").ToString());

                                    if (itemValue5 > 0)
                                    {
                                        propertyInfo5.SetValue(ado, itemValue5.ToString());
                                    }
                                }
                                PropertyInfo propertyInfo6 = ps.FirstOrDefault(o => o.Name == string.Format("SUM_Day{0}", j + 1));
                                if (propertyInfo6 != null)
                                {
                                    decimal sum_Day = itemValue2 + itemValue3 + itemValue4 + itemValue5;
                                    if (sum_Day > 0)
                                    {
                                        propertyInfo6.SetValue(ado, sum_Day.ToString());
                                    }
                                }
                            }
                        }

                        result.Add(ado);
                    }
                }
            }

            return result;
        }
    }



    public class ServiceTypeADO
    {
        public long Service_Type_Id { get; set; }
        public string TYPE_NAME { get; set; }
    }
}
