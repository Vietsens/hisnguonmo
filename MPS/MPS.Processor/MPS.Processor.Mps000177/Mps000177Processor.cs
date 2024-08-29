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
using MPS.Processor.Mps000177.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000177
{
    public class Mps000177Processor : AbstractProcessor
    {
        Mps000177PDO rdo;
        List<Mps000177MediMate> medicine = null;
        List<Mps000177MediMate> material = null;
        List<PatientADO> lstAdo = new List<PatientADO>();
        List<EMMedicine> emMedicines = new List<EMMedicine>();
        List<EMMaterial> emMaterials = new List<EMMaterial>();
        List<EMBlood> emBloods = new List<EMBlood>();
        public Mps000177Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000177PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = true;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetData();
                ProcessDataByDaySize();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "list", rdo.currentPatient);
                objectTag.AddObjectData(store, "medicine", medicine);
                objectTag.AddObjectData(store, "material", material);
                objectTag.AddRelationship(store, "list", "medicine", "treatment_id", "treatment_id");
                objectTag.AddRelationship(store, "list", "material", "treatment_id", "treatment_id");


                objectTag.AddObjectData(store, "ListTreatmentPrint", lstAdo);
                objectTag.AddObjectData(store, "MedicineByDay", emMedicines);
                objectTag.AddObjectData(store, "MaterialByDay", emMaterials);
                objectTag.AddObjectData(store, "BloodByDay", emBloods);
                objectTag.AddRelationship(store, "ListTreatmentPrint", "MedicineByDay", new string[] { "treatment_id", "Page" }, new string[] { "treatment_id", "Page" });
                objectTag.AddRelationship(store, "ListTreatmentPrint", "MaterialByDay", new string[] { "treatment_id", "Page" }, new string[] { "treatment_id", "Page" });
                objectTag.AddRelationship(store, "ListTreatmentPrint", "BloodByDay", new string[] { "treatment_id", "Page" }, new string[] { "treatment_id", "Page" });

                objectTag.SetUserFunction(store, "FGetDicValue", new FGetDicValue());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void ProcessDataByDaySize()
        {

            try
            {
                rdo.DaySize = rdo.DaySize > 0 ? rdo.DaySize : Int64.MaxValue;
                ProcessMedicines();
                ProcessMaterials();
                ProcessBloods();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ProcessMaterials()
        {
            try
            {
                if (rdo.VExpMestMaterial != null && rdo.VExpMestMaterial.Count > 0)
                {
                    rdo.VExpMestMaterial = rdo.VExpMestMaterial.OrderBy(o => o.TDL_INTRUCTION_TIME).ToList();
                    var groupByTreatment = rdo.VExpMestMaterial.GroupBy(o => new { o.TDL_TREATMENT_ID, o.TDL_MATERIAL_TYPE_ID });
                    foreach (var item in groupByTreatment)
                    {
                        var materials = item.ToList().OrderBy(o => o.TDL_INTRUCTION_TIME).ToList();
                        var lstByTreatment = lstAdo.Where(o => o.treatment_id == item.Key.TDL_TREATMENT_ID).ToList();
                        if (lstByTreatment == null || lstByTreatment.Count == 0)
                        {
                            PatientADO ado = new PatientADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<PatientADO>(ado, rdo.currentPatient.FirstOrDefault(o => o.treatment_id == item.Key.TDL_TREATMENT_ID));
                            ado.treatment_id = item.Key.TDL_TREATMENT_ID ?? 0;
                            if (rdo.bedRoomName.ContainsKey(ado.treatment_id))
                            {
                                ado.ROOM_NAME = rdo.bedRoomName[ado.treatment_id].BED_ROOM_NAME;
                                ado.BED_NAME = rdo.bedRoomName[ado.treatment_id].BED_NAME;
                            }
                            ado.DicMediDay = new Dictionary<long, long>();
                            ado.DicMateDay = new Dictionary<long, long>();
                            ado.DicBloodDay = new Dictionary<long, long>();
                            ado.Page = 1;
                            lstAdo.Add(ado);
                            lstByTreatment.Add(ado);
                        }
                        foreach (var me in materials)
                        {
                            var ado = lstByTreatment.FirstOrDefault(o => o.DicMateDay.ContainsValue(me.TDL_INTRUCTION_DATE ?? 0));
                            if (ado == null)
                            {
                                ado = lstByTreatment.FirstOrDefault(o => o.DicMateDay.Keys.Count < rdo.DaySize);
                                if (ado == null)
                                {
                                    ado = new PatientADO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<PatientADO>(ado, rdo.currentPatient.FirstOrDefault(o => o.treatment_id == item.Key.TDL_TREATMENT_ID));
                                    ado.treatment_id = item.Key.TDL_TREATMENT_ID ?? 0;
                                    if (rdo.bedRoomName.ContainsKey(ado.treatment_id))
                                    {
                                        ado.ROOM_NAME = rdo.bedRoomName[ado.treatment_id].BED_ROOM_NAME;
                                        ado.BED_NAME = rdo.bedRoomName[ado.treatment_id].BED_NAME;
                                    }
                                    ado.DicMediDay = new Dictionary<long, long>();
                                    ado.DicMateDay = new Dictionary<long, long>();
                                    ado.DicBloodDay = new Dictionary<long, long>();
                                    ado.Page = lstByTreatment.Count + 1;
                                    ado.DicMateDay.Add(1, me.TDL_INTRUCTION_DATE ?? 0);
                                    lstAdo.Add(ado);
                                    lstByTreatment.Add(ado);
                                }
                                else
                                {
                                    ado.DicMateDay.Add(ado.DicMateDay.Keys.Count + 1, me.TDL_INTRUCTION_DATE ?? 0);
                                    //lstByTreatment = lstAdo.Where(o => o.treatment_id == item.Key.TDL_TREATMENT_ID).ToList();
                                }
                            }
                            var emt = emMaterials.FirstOrDefault(o => o.treatment_id == me.TDL_TREATMENT_ID && o.TDL_MATERIAL_TYPE_ID == me.TDL_MATERIAL_TYPE_ID && o.Page == ado.Page);
                            if (emt != null)
                            {
                                if (emt.DicAmount.ContainsKey(me.TDL_INTRUCTION_DATE ?? 0))
                                {
                                    emt.DicAmount[me.TDL_INTRUCTION_DATE ?? 0] += me.AMOUNT;
                                }
                                else
                                {
                                    emt.DicAmount.Add(me.TDL_INTRUCTION_DATE ?? 0, me.AMOUNT);
                                }
                            }
                            else
                            {
                                EMMaterial emm = new EMMaterial();
                                Inventec.Common.Mapper.DataObjectMapper.Map<EMMaterial>(emm, me);
                                emm.treatment_id = item.Key.TDL_TREATMENT_ID ?? 0;
                                emm.DicAmount = new Dictionary<long, decimal?>();
                                emm.DicAmount.Add(me.TDL_INTRUCTION_DATE ?? 0, me.AMOUNT);
                                emm.Page = ado.Page;
                                emMaterials.Add(emm);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ProcessBloods()
        {
            try
            {
                if (rdo.VExpMestBlood != null && rdo.VExpMestBlood.Count > 0)
                {
                    rdo.VExpMestBlood = rdo.VExpMestBlood.OrderBy(o => o.TDL_INTRUCTION_TIME).ToList();
                    var groupByTreatment = rdo.VExpMestBlood.GroupBy(o => new { o.TDL_TREATMENT_ID, o.TDL_BLOOD_TYPE_ID });
                    foreach (var item in groupByTreatment)
                    {
                        var bloods = item.ToList().OrderBy(o => o.TDL_INTRUCTION_TIME).ToList();
                        var lstByTreatment = lstAdo.Where(o => o.treatment_id == item.Key.TDL_TREATMENT_ID).ToList();
                        if (lstByTreatment == null || lstByTreatment.Count == 0)
                        {
                            PatientADO ado = new PatientADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<PatientADO>(ado, rdo.currentPatient.FirstOrDefault(o => o.treatment_id == item.Key.TDL_TREATMENT_ID));
                            ado.treatment_id = item.Key.TDL_TREATMENT_ID ?? 0;
                            if (rdo.bedRoomName.ContainsKey(ado.treatment_id))
                            {
                                ado.ROOM_NAME = rdo.bedRoomName[ado.treatment_id].BED_ROOM_NAME;
                                ado.BED_NAME = rdo.bedRoomName[ado.treatment_id].BED_NAME;
                            }
                            ado.DicMediDay = new Dictionary<long, long>();
                            ado.DicMateDay = new Dictionary<long, long>();
                            ado.DicBloodDay = new Dictionary<long, long>();
                            ado.Page = 1;
                            lstAdo.Add(ado);
                            lstByTreatment.Add(ado);
                        }
                        foreach (var me in bloods)
                        {
                            var ado = lstByTreatment.FirstOrDefault(o => o.DicBloodDay.ContainsValue(me.TDL_INTRUCTION_DATE ?? 0));
                            if (ado == null)
                            {
                                ado = lstByTreatment.FirstOrDefault(o => o.DicBloodDay.Keys.Count < rdo.DaySize);
                                if (ado == null)
                                {
                                    ado = new PatientADO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<PatientADO>(ado, rdo.currentPatient.FirstOrDefault(o => o.treatment_id == item.Key.TDL_TREATMENT_ID));
                                    ado.treatment_id = item.Key.TDL_TREATMENT_ID ?? 0;
                                    if (rdo.bedRoomName.ContainsKey(ado.treatment_id))
                                    {
                                        ado.ROOM_NAME = rdo.bedRoomName[ado.treatment_id].BED_ROOM_NAME;
                                        ado.BED_NAME = rdo.bedRoomName[ado.treatment_id].BED_NAME;
                                    }
                                    ado.DicMediDay = new Dictionary<long, long>();
                                    ado.DicMateDay = new Dictionary<long, long>();
                                    ado.DicBloodDay = new Dictionary<long, long>();
                                    ado.Page = lstByTreatment.Count + 1;
                                    ado.DicBloodDay.Add(1, me.TDL_INTRUCTION_DATE ?? 0);
                                    lstAdo.Add(ado);
                                    lstByTreatment.Add(ado);
                                }
                                else
                                {
                                    ado.DicBloodDay.Add(ado.DicBloodDay.Keys.Count + 1, me.TDL_INTRUCTION_DATE ?? 0);
                                }
                            }
                            var emt = emBloods.FirstOrDefault(o => o.treatment_id == me.TDL_TREATMENT_ID && o.TDL_BLOOD_TYPE_ID == me.TDL_BLOOD_TYPE_ID && o.Page == ado.Page);
                            if (emt != null)
                            {
                                if (emt.DicAmount.ContainsKey(me.TDL_INTRUCTION_DATE ?? 0))
                                {
                                    emt.DicAmount[me.TDL_INTRUCTION_DATE ?? 0] += 1;
                                }
                                else
                                {
                                    emt.DicAmount.Add(me.TDL_INTRUCTION_DATE ?? 0, 1); 
                                }
                            }
                            else
                            {
                                EMBlood emb = new EMBlood();
                                Inventec.Common.Mapper.DataObjectMapper.Map<EMBlood>(emb, me);
                                emb.treatment_id = item.Key.TDL_TREATMENT_ID ?? 0;
                                emb.DicAmount = new Dictionary<long, decimal?>();
                                emb.DicAmount.Add(me.TDL_INTRUCTION_DATE ?? 0, 1);
                                emb.Page = ado.Page;
                                emBloods.Add(emb);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ProcessMedicines()
        {
            try
            {
                if (rdo.VExpMestMedicine != null && rdo.VExpMestMedicine.Count > 0)
                {
                    rdo.VExpMestMedicine = rdo.VExpMestMedicine.OrderBy(o => o.TDL_INTRUCTION_TIME).ToList();
                    var groupByTreatment = rdo.VExpMestMedicine.GroupBy(o => new { o.TDL_TREATMENT_ID, o.TDL_MEDICINE_TYPE_ID });
                    foreach (var item in groupByTreatment)
                    {
                        var medicines = item.ToList().OrderBy(o => o.TDL_INTRUCTION_TIME).ToList();
                        var lstByTreatment = lstAdo.Where(o => o.treatment_id == item.Key.TDL_TREATMENT_ID).ToList();
                        if (lstByTreatment == null || lstByTreatment.Count == 0)
                        {
                            PatientADO ado = new PatientADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<PatientADO>(ado, rdo.currentPatient.FirstOrDefault(o => o.treatment_id == item.Key.TDL_TREATMENT_ID));
                            ado.treatment_id = item.Key.TDL_TREATMENT_ID ?? 0;
                            if (rdo.bedRoomName.ContainsKey(ado.treatment_id))
                            {
                                ado.ROOM_NAME = rdo.bedRoomName[ado.treatment_id].BED_ROOM_NAME;
                                ado.BED_NAME = rdo.bedRoomName[ado.treatment_id].BED_NAME;
                            }
                            ado.DicMediDay = new Dictionary<long, long>();
                            ado.DicMateDay = new Dictionary<long, long>();
                            ado.DicBloodDay = new Dictionary<long, long>();
                            ado.Page = 1;
                            lstAdo.Add(ado);
                            lstByTreatment.Add(ado);
                        }
                        foreach (var me in medicines)
                        {
                            var ado = lstByTreatment.FirstOrDefault(o => o.DicMediDay.ContainsValue(me.TDL_INTRUCTION_DATE ?? 0));
                            if (ado == null)
                            {
                                ado = lstByTreatment.FirstOrDefault(o => o.DicMediDay.Keys.Count < rdo.DaySize);
                                if(ado == null)
                                {
                                    ado = new PatientADO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<PatientADO>(ado, rdo.currentPatient.FirstOrDefault(o => o.treatment_id == item.Key.TDL_TREATMENT_ID));
                                    ado.treatment_id = item.Key.TDL_TREATMENT_ID ?? 0;
                                    if (rdo.bedRoomName.ContainsKey(ado.treatment_id))
                                    {
                                        ado.ROOM_NAME = rdo.bedRoomName[ado.treatment_id].BED_ROOM_NAME;
                                        ado.BED_NAME = rdo.bedRoomName[ado.treatment_id].BED_NAME;
                                    }
                                    ado.DicMediDay = new Dictionary<long, long>();
                                    ado.DicMateDay = new Dictionary<long, long>();
                                    ado.DicBloodDay = new Dictionary<long, long>();
                                    ado.Page = lstByTreatment.Count + 1;
                                    ado.DicMediDay.Add(1, me.TDL_INTRUCTION_DATE ?? 0);
                                    lstAdo.Add(ado);
                                    lstByTreatment.Add(ado);
                                }
                                else
                                {
                                    ado.DicMediDay.Add(ado.DicMediDay.Keys.Count + 1, me.TDL_INTRUCTION_DATE ?? 0);
                                    //var Patient = lstAdo.LastOrDefault(o => o.treatment_id == ado.treatment_id);
                                    //Patient.DicMediDay = ado.DicMediDay;
                                    //lstByTreatment = lstAdo.Where(o => o.treatment_id == item.Key.TDL_TREATMENT_ID).ToList();
                                } 
                            }
                            var emt = emMedicines.FirstOrDefault(o => o.treatment_id == me.TDL_TREATMENT_ID && o.TDL_MEDICINE_TYPE_ID == me.TDL_MEDICINE_TYPE_ID && o.Page == ado.Page);
                            if (emt != null)
                            {
                                if (emt.DicAmount.ContainsKey(me.TDL_INTRUCTION_DATE ?? 0))
                                {
                                    emt.DicAmount[me.TDL_INTRUCTION_DATE ?? 0] += me.AMOUNT;
                                }
                                else
                                {
                                    emt.DicAmount.Add(me.TDL_INTRUCTION_DATE ?? 0, me.AMOUNT);
                                }
                            }
                            else
                            {
                                EMMedicine emm = new EMMedicine();
                                Inventec.Common.Mapper.DataObjectMapper.Map<EMMedicine>(emm, me);
                                emm.treatment_id = item.Key.TDL_TREATMENT_ID ?? 0;
                                emm.DicAmount = new Dictionary<long, decimal?>();
                                emm.DicAmount.Add(me.TDL_INTRUCTION_DATE ?? 0, me.AMOUNT);
                                emm.Page = ado.Page;
                                emMedicines.Add(emm);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void SetData()
        {
            try
            {
                if (rdo.Mps000177MediMate != null && rdo.Mps000177MediMate.Count > 0)
                {
                    medicine = new List<Mps000177MediMate>();
                    material = new List<Mps000177MediMate>();
                    foreach (var item in rdo.Mps000177MediMate)
                    {
                        if (item.type == 1)
                        {
                            medicine.Add(item);
                        }
                        else
                        {
                            material.Add(item);
                        }
                    }
                }

                if (rdo.Mps000177DAY != null && rdo.Mps000177DAY.Count > 0)
                {
                    foreach (var patient in rdo.currentPatient)
                    {
                        if (rdo.bedRoomName.ContainsKey(patient.treatment_id))
                        {
                            patient.ROOM_NAME = rdo.bedRoomName[patient.treatment_id].BED_ROOM_NAME;
                            patient.BED_NAME = rdo.bedRoomName[patient.treatment_id].BED_NAME;
                        }
                        var item = rdo.Mps000177DAY.FirstOrDefault(o => o.treatment_id == patient.treatment_id);
                        if (item == null) continue;
                        patient.Day1 = item.Day1;
                        patient.Day2 = item.Day2;
                        patient.Day3 = item.Day3;
                        patient.Day4 = item.Day4;
                        patient.Day5 = item.Day5;
                        patient.Day6 = item.Day6;
                        patient.Day7 = item.Day7;
                        patient.Day8 = item.Day8;
                        patient.Day9 = item.Day9;
                        patient.Day10 = item.Day10;
                        patient.Day11 = item.Day11;
                        patient.Day12 = item.Day12;
                        patient.Day13 = item.Day13;
                        patient.Day14 = item.Day14;
                        patient.Day15 = item.Day15;
                        patient.Day16 = item.Day16;
                        patient.Day17 = item.Day17;
                        patient.Day18 = item.Day18;
                        patient.Day19 = item.Day19;
                        patient.Day20 = item.Day20;
                        patient.Day21 = item.Day21;
                        patient.Day22 = item.Day22;
                        patient.Day23 = item.Day23;
                        patient.Day24 = item.Day24;
                    }
                }

                if (!String.IsNullOrEmpty(rdo.departmentName))
                {
                    SetSingleKey(new KeyValue(Mps000177ExtendSingleKey.DEPARTMENT_NAME, rdo.departmentName));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class FGetDicValue : TFlexCelUserFunction
    {

        object result = null;
        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length < 2)
                throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");


            try
            {
                string listKey = Convert.ToString(parameters[1]);
                if (string.IsNullOrWhiteSpace(listKey))
                {
                    listKey = "";
                }
                string[] arrayKey = listKey.Split(',');
                if (parameters[0] is Dictionary<string, int>)
                {
                    Dictionary<string, int> DicGet = parameters[0] as Dictionary<string, int>;
                    result = DicGet.Where(o => arrayKey.Contains(o.Key)).Sum(p => p.Value);
                }
                else if (parameters[0] is Dictionary<string, long>)
                {
                    Dictionary<string, long> DicGet = parameters[0] as Dictionary<string, long>;
                    result = DicGet.Where(o => arrayKey.Contains(o.Key)).Sum(p => p.Value);
                }
                else if (parameters[0] is Dictionary<string, decimal>)
                {
                    Dictionary<string, decimal> DicGet = parameters[0] as Dictionary<string, decimal>;
                    result = DicGet.Where(o => arrayKey.Contains(o.Key)).Sum(p => p.Value);
                }
                else if (parameters[0] is Dictionary<string, string>)
                {
                    Dictionary<string, string> DicGet = parameters[0] as Dictionary<string, string>;
                    result = string.Join(";", DicGet.Where(o => arrayKey.Contains(o.Key)).Select(p => p.Value).ToList());
                }
                else if (parameters[0] is Dictionary<long, long>)
                {
                    Dictionary<long, long> DicGet = parameters[0] as Dictionary<long, long>;
                    result = DicGet.Where(o => arrayKey.Contains(o.Key.ToString())).Sum(p => p.Value);
                }
                else if (parameters[0] is Dictionary<long, decimal?>)
                {
                    Dictionary<long, decimal?> DicGet = parameters[0] as Dictionary<long, decimal?>;
                    var chkList = DicGet.Where(o => arrayKey.Contains(o.Key.ToString()) && o.Value.HasValue);
                    result = chkList != null && chkList.Count() > 0 ? chkList.Sum(p => p.Value) : null;
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }

            return result;
        }
    }
}
