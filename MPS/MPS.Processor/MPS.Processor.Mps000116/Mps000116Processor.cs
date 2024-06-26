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
using MPS.Processor.Mps000116.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000116
{
    public class Mps000116Processor : AbstractProcessor
    {
        Mps000116PDO rdo;
        public List<Mps000116ADO> medicinePublicByDates { get; set; }
        List<Type> _Types = null;

        public Mps000116Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000116PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {

                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                //Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                //barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "Type", _Types);
                objectTag.AddObjectData(store, "MedicinesADO", rdo._Mps000116ADOs);
                objectTag.AddRelationship(store, "Type", "MedicinesADO", "ID", "TypeId");
                objectTag.SetUserFunction(store, "FuncMergeData11", new CalculateMergerData());

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
                _Types = new List<Type>();
                if (rdo._Mps000116ADOs != null && rdo._Mps000116ADOs.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Error("rdo._Mps000116ADOs_________________." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo._Mps000116ADOs), rdo._Mps000116ADOs));

                    List<long> _type = rdo._Mps000116ADOs.Select(p => p.TypeId).Distinct().ToList();
                    foreach (var item in _type)
                    {
                        if (item == 1)
                        {
                            Type ado = new Type();
                            ado.SERVICE_TYPE_NAME = "THUỐC";
                            ado.ID = item;
                            _Types.Add(ado);
                        }
                        else if (item == 2)
                        {
                            Type ado = new Type();
                            ado.SERVICE_TYPE_NAME = "VẬT TƯ";
                            ado.ID = item;
                            _Types.Add(ado);
                        }
                    }
                    _Types = _Types.OrderBy(p => p.SERVICE_TYPE_NAME).ToList();
                }
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo._Treatment, false);
                AddObjectKeyIntoListkey<V_HIS_BED_LOG>(rdo._vHisBedLog, false);

                if (rdo._Treatment.CLINICAL_IN_TIME != null)
                {
                    SetSingleKey(new KeyValue(Mps000116ExtendSingleKey.IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((rdo._Treatment.CLINICAL_IN_TIME ?? 0))));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000116ExtendSingleKey.IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((rdo._Treatment.IN_TIME))));
                }
                if (rdo._Treatment.CLINICAL_IN_TIME != null)
                {
                    SetSingleKey(new KeyValue(Mps000116ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((rdo._Treatment.CLINICAL_IN_TIME ?? 0))));
                }
                if (rdo._Treatment.OUT_TIME != null)
                {
                    SetSingleKey(new KeyValue(Mps000116ExtendSingleKey.OUT_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((rdo._Treatment.OUT_TIME ?? 0))));
                }
                if (rdo._WorkPlace != null)
                {
                    AddObjectKeyIntoListkey<MOS.SDO.WorkPlaceSDO>(rdo._WorkPlace, false);
                }
                if (rdo._SingleKeys != null)
                {
                    AddObjectKeyIntoListkey<SingleKeys>(rdo._SingleKeys, false);
                }
                if (rdo._Mps000116ADOs != null && rdo._Mps000116ADOs.Count > 0 && rdo._SingleKeys.IsOderMedicine == 1)
                {
                     rdo._Mps000116ADOs = rdo._Mps000116ADOs
                    .OrderBy(o => o.SERVICE_TYPE_ID)
                    .ThenByDescending(o => o.MEDICINE_GROUP_NUM_ORDER)
                    .ThenByDescending(o => o.MEDICINE_USE_FORM_NUM_ORDER)
                    .ThenBy(o => o.NUM_ORDER)
                    .ThenBy(o => o.MEDI_MATY_TYPE_NAME)
                    .ToList();
                }
                Inventec.Common.Logging.LogSystem.Debug("_mps000116ADOs______________" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo._Mps000116ADOs), rdo._Mps000116ADOs));

                SetSingleKey(new KeyValue(Mps000116ExtendSingleKey.INTRUCTION_TIME_DAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo._IntructionTime))));
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    class Type
    {
        public string SERVICE_TYPE_NAME { get; set; }
        public long ID { get; set; }
    }

    class CalculateMergerData : TFlexCelUserFunction
    {
        long typeId = 0;

        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
            bool result = false;
            try
            {
                long servicetypeId = Convert.ToInt64(parameters[0]);

                if (servicetypeId > 0)
                {
                    if (this.typeId == servicetypeId)
                    {
                        return true;
                    }
                    else
                    {
                        this.typeId = servicetypeId;
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
