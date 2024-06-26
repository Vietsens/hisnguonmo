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
using MPS.Processor.Mps000274.ADO;
using MPS.Processor.Mps000274.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000274
{
    public class Mps000274Processor : AbstractProcessor
    {
        Mps000274PDO rdo;
        List<SereServADO> Output;
        List<SereServXAADO> SereServXAADOs;
        Dictionary<long, string> dicTreatmentBedRoomNow;
        List<SereServADO> SereServGroups;
        List<SereServXAADO> SereServXAADOGroups;
        List<HIS_RATION_TIME> Rations;

        List<SereServADO> SereServDetail;

        List<TotalADO> listGroupDepartment = new List<TotalADO>();
        List<TotalADO> listGroupPaty = new List<TotalADO>();
        List<TotalADO> listGroupPatyTotal = new List<TotalADO>();
        Dictionary<long, V_HIS_TREATMENT_BED_ROOM> dicBedRoomNow = new Dictionary<long, V_HIS_TREATMENT_BED_ROOM>();

        public Mps000274Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000274PDO)rdoBase;
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                ProcessSereServ();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                ProcessTotalData();

                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "SereServ", Output);
                objectTag.AddObjectData(store, "SereServXA", SereServXAADOs);
                objectTag.AddObjectData(store, "SereServGroups", SereServGroups);
                objectTag.AddObjectData(store, "SereServXAADOGroups", SereServXAADOGroups);
                objectTag.AddObjectData(store, "Rations", Rations.OrderBy(o => o.ID).ToList());
                objectTag.AddObjectData(store, "RationXAADO", Rations.OrderBy(o => o.ID).ToList());
                objectTag.AddRelationship(store, "Rations", "SereServGroups", "ID", "RATION_TIME_ID");
                objectTag.AddRelationship(store, "RationXAADO", "SereServXAADOGroups", "ID", "RATION_TIME_ID");

                objectTag.AddObjectData(store, "SereServDetail", SereServDetail);

                objectTag.AddObjectData(store, "Department", listGroupDepartment);
                objectTag.AddObjectData(store, "SuatAn", listGroupPaty);
                objectTag.AddRelationship(store, "Department", "SuatAn", "DEPARTMENT_ID", "DEPARTMENT_ID");
                objectTag.SetUserFunction(this.store, "FlFuncElement", new FlFuncElementFunction());

                objectTag.AddObjectData(store, "GroupPatyTotal", listGroupPatyTotal);

                barCodeTag.ProcessData(store, dicImage);

                store.SetCommonFunctions();

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ProcessTotalData()
        {
            try
            {
                if (rdo.ListSereServ == null || rdo.ListSereServ.Count <= 0)
                {
                    return;
                }

                listGroupDepartment = new List<TotalADO>();
                listGroupPaty = new List<TotalADO>();
                listGroupPatyTotal = new List<TotalADO>();

                Dictionary<string, string> dicName = new Dictionary<string, string>();

                List<string> patientTypes = rdo.ListSereServ.Select(s => s.TDL_SERVICE_NAME).Distinct().OrderBy(o => o).ToList();
                if (patientTypes != null && patientTypes.Count > 0)
                {
                    int count = 1;
                    foreach (var item in patientTypes)
                    {
                        SetSingleKey(new KeyValue("COL_SERVICE_NAME_" + count, item));
                        dicName.Add(item, "COL_SERVICE_NAME_" + count);
                        count++;
                    }
                }

                var groupDepartment = rdo.ListSereServ.GroupBy(o => o.TDL_REQUEST_DEPARTMENT_ID).ToList();
                foreach (var department in groupDepartment)
                {
                    TotalADO depa = new TotalADO();
                    depa.DEPARTMENT_CODE = department.First().REQUEST_DEPARTMENT_CODE;
                    depa.DEPARTMENT_NAME = department.First().REQUEST_DEPARTMENT_NAME;
                    depa.DEPARTMENT_ID = department.First().TDL_REQUEST_DEPARTMENT_ID;
                    depa.AMOUNT = department.Sum(s => s.AMOUNT);

                    depa.DicData = new Dictionary<string, decimal>();
                    var groupDepaByName = department.GroupBy(o => o.TDL_SERVICE_NAME).ToList();
                    foreach (var item in groupDepaByName)
                    {
                        if (dicName.ContainsKey(item.Key))
                        {
                            depa.DicData.Add(dicName[item.Key], item.Sum(s => s.AMOUNT));
                        }
                    }

                    listGroupDepartment.Add(depa);

                    var groupPatientType = department.GroupBy(o => o.PATIENT_TYPE_ID).ToList();
                    foreach (var patientType in groupPatientType)
                    {
                        TotalADO pa = new TotalADO();
                        pa.DEPARTMENT_CODE = department.First().REQUEST_DEPARTMENT_CODE;
                        pa.DEPARTMENT_NAME = department.First().REQUEST_DEPARTMENT_NAME;
                        pa.DEPARTMENT_ID = department.First().TDL_REQUEST_DEPARTMENT_ID;
                        pa.AMOUNT = patientType.Sum(s => s.AMOUNT);

                        if (rdo.ListPatientType != null && rdo.ListPatientType.Count > 0)
                        {
                            var paty = rdo.ListPatientType.FirstOrDefault(o => o.ID == patientType.Key);
                            if (paty != null)
                            {
                                pa.PATIENT_TYPE_CODE = paty.PATIENT_TYPE_CODE;
                                pa.PATIENT_TYPE_NAME = paty.PATIENT_TYPE_NAME;
                            }
                        }

                        pa.DicData = new Dictionary<string, decimal>();
                        var groupPatyByName = patientType.GroupBy(o => o.TDL_SERVICE_NAME).ToList();
                        foreach (var item in groupPatyByName)
                        {
                            if (dicName.ContainsKey(item.Key))
                            {
                                pa.DicData.Add(dicName[item.Key], item.Sum(s => s.AMOUNT));
                            }
                        }

                        listGroupPaty.Add(pa);
                    }
                }

                var groupPaty = rdo.ListSereServ.GroupBy(o => o.PATIENT_TYPE_ID).ToList();
                foreach (var item in groupPaty)
                {
                    TotalADO pa = new TotalADO();
                    pa.AMOUNT = item.Sum(s => s.AMOUNT);
                    if (rdo.ListPatientType != null && rdo.ListPatientType.Count > 0)
                    {
                        var paty = rdo.ListPatientType.FirstOrDefault(o => o.ID == item.Key);
                        if (paty != null)
                        {
                            pa.PATIENT_TYPE_CODE = paty.PATIENT_TYPE_CODE;
                            pa.PATIENT_TYPE_NAME = paty.PATIENT_TYPE_NAME;
                        }
                    }

                    listGroupPatyTotal.Add(pa);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSereServ()
        {
            try
            {
                if (rdo.ListSereServ == null || rdo.ListSereServ.Count <= 0)
                {
                    return;
                }

                Output = new List<SereServADO>();
                SereServXAADOs = new List<SereServXAADO>();
                SereServGroups = new List<SereServADO>();
                SereServXAADOGroups = new List<SereServXAADO>();
                dicTreatmentBedRoomNow = new Dictionary<long, string>();
                Rations = new List<HIS_RATION_TIME>();

                //lay ds ration time 
                if (rdo.ListRationTime != null && rdo.ListRationTime.Count > 0)
                {
                    var rationGroup = rdo.ListSereServ.GroupBy(o => o.RATION_TIME_ID);
                    foreach (var rationItem in rationGroup)
                    {
                        var data = rationItem.FirstOrDefault();
                        HIS_RATION_TIME rationtime = new HIS_RATION_TIME();
                        rationtime = rdo.ListRationTime.FirstOrDefault(o => o.ID == data.RATION_TIME_ID);
                        Rations.Add(rationtime);
                    }
                }

                var groupSereServ = rdo.ListSereServ.GroupBy(o => new { o.TDL_PATIENT_ID, o.SERVICE_ID, o.PRICE, o.INSTRUCTION_NOTE }).ToList();

                SetSingleKey(new KeyValue(Mps000274ExtendSingleKey.REQUEST_DEPARTMENT_NAME, rdo.ListSereServ.FirstOrDefault().REQUEST_DEPARTMENT_NAME));

                foreach (var group in groupSereServ)
                {
                    var fistGroup = group.FirstOrDefault();
                    SereServADO Parent = new SereServADO(fistGroup);
                    Parent.TITLE_NAME = fistGroup.REQUEST_DEPARTMENT_NAME;
                    Parent.AMOUNT_SUM = group.Sum(o => o.AMOUNT);
                    Parent.BED_ROOM_NAME__BED_NAME = GetTreatmentBedRoomByPatient(fistGroup.TDL_TREATMENT_ID ?? 0);
                    Output.Add(Parent);
                    //Gom nhom theo RATION_TIME_ID
                    var dataGroup = group.GroupBy(o => o.RATION_TIME_ID).ToList();
                    foreach (var item in dataGroup)
                    {
                        var itemFist = item.FirstOrDefault();
                        SereServADO SereServItem = new SereServADO(itemFist);
                        SereServItem.TITLE_NAME = itemFist.REQUEST_DEPARTMENT_NAME;
                        SereServItem.AMOUNT_SUM = item.Sum(o => o.AMOUNT);
                        SereServItem.BED_ROOM_NAME__BED_NAME = GetTreatmentBedRoomByPatient(fistGroup.TDL_TREATMENT_ID ?? 0);
                        SereServGroups.Add(SereServItem);
                    }
                }

                var groupSereServXA = rdo.ListSereServ.GroupBy(o => new { o.TDL_SERVICE_NAME }).ToList();
                foreach (var group in groupSereServXA)
                {
                    var fistGroup = group.FirstOrDefault();
                    SereServXAADO Parent = new SereServXAADO();
                    Parent.NAME = fistGroup.TDL_SERVICE_NAME;
                    Parent.AMOUNT = group.Sum(o => o.AMOUNT);
                    Parent.DESCRIPTION = "";

                    SereServXAADOs.Add(Parent);

                    //Gom nhom theo RATION_TIME_ID
                    var dataGroup = group.GroupBy(o => o.RATION_TIME_ID).ToList();
                    foreach (var item in dataGroup)
                    {
                        var itemFist = item.FirstOrDefault();
                        SereServXAADO SereServItem = new SereServXAADO();
                        SereServItem.RATION_TIME_ID = itemFist.RATION_TIME_ID;
                        SereServItem.NAME = itemFist.TDL_SERVICE_NAME;
                        SereServItem.AMOUNT = item.Sum(o => o.AMOUNT);
                        SereServItem.DESCRIPTION = "";
                        SereServXAADOGroups.Add(SereServItem);
                    }
                }

                SereServDetail = new List<SereServADO>();
                var groupSereServPaty = rdo.ListSereServ.GroupBy(o => new { o.TDL_TREATMENT_ID, o.PATIENT_TYPE_ID, o.SERVICE_ID, o.INSTRUCTION_NOTE, o.RATION_TIME_ID }).ToList();
                foreach (var item in groupSereServPaty)
                {
                    var itemFist = item.First();
                    SereServADO SereServItem = new SereServADO(itemFist);
                    SereServItem.TITLE_CODE = itemFist.REQUEST_DEPARTMENT_CODE;
                    SereServItem.TITLE_NAME = itemFist.REQUEST_DEPARTMENT_NAME;
                    SereServItem.AMOUNT_SUM = item.Sum(o => o.AMOUNT);

                    var bedRoom = GetLastBedRoomByTreatment(itemFist.TDL_TREATMENT_ID ?? 0);
                    if (bedRoom != null)
                    {
                        SereServItem.BED_ROOM_NAME__BED_NAME = bedRoom.BED_ROOM_NAME + (!String.IsNullOrEmpty(bedRoom.BED_NAME) ? " - " + bedRoom.BED_NAME : "");
                        SereServItem.BED_CODE = bedRoom.BED_CODE;
                        SereServItem.BED_NAME = bedRoom.BED_NAME;
                        SereServItem.BED_ROOM_CODE = bedRoom.BED_ROOM_CODE;
                        SereServItem.BED_ROOM_NAME = bedRoom.BED_ROOM_NAME;
                    }

                    if (rdo.ListPatientType != null && rdo.ListPatientType.Count > 0)
                    {
                        var paty = rdo.ListPatientType.FirstOrDefault(o => o.ID == itemFist.PATIENT_TYPE_ID);
                        if (paty != null)
                        {
                            SereServItem.PATIENT_TYPE_NAME = paty.PATIENT_TYPE_NAME;
                        }
                    }

                    if (rdo.ListTreatments != null && rdo.ListTreatments.Count > 0)
                    {
                        var treatment = rdo.ListTreatments.FirstOrDefault(o => o.ID == itemFist.TDL_TREATMENT_ID);
                        if (treatment != null)
                        {
                            SereServItem.PATIENT_CLASSIFY_NAME = treatment.TDL_PATIENT_CLASSIFY_NAME;
                        }
                    }

                    SereServDetail.Add(SereServItem);
                }

                SereServDetail = SereServDetail.OrderBy(o => o.TDL_TREATMENT_CODE).ThenBy(o => o.INTRUCTION_TIME).ThenBy(o => o.ID).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_TREATMENT_BED_ROOM GetLastBedRoomByTreatment(long treatmentId)
        {
            V_HIS_TREATMENT_BED_ROOM result = null;
            try
            {
                if (dicBedRoomNow.ContainsKey(treatmentId))
                {
                    dicBedRoomNow.TryGetValue(treatmentId, out result);
                    if (result != null)
                        return result;
                }

                if (rdo.ListTreatmentBedRooms != null)
                {
                    List<V_HIS_TREATMENT_BED_ROOM> TreatmentBedRoom = rdo.ListTreatmentBedRooms.Where(o => o.TREATMENT_ID == treatmentId).OrderByDescending(o => o.ADD_TIME).ToList();

                    result = (TreatmentBedRoom != null && TreatmentBedRoom.Count > 0) ? TreatmentBedRoom.First() : null;
                }

                if (result != null)
                    dicBedRoomNow[treatmentId] = result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        string GetTreatmentBedRoomByPatient(long treatmentId)
        {
            string dataResult = String.Empty;
            try
            {
                if (dicTreatmentBedRoomNow.ContainsKey(treatmentId))
                {
                    dicTreatmentBedRoomNow.TryGetValue(treatmentId, out dataResult);
                    return dataResult;
                }

                if (rdo.ListTreatmentBedRooms != null)
                {
                    List<V_HIS_TREATMENT_BED_ROOM> TreatmentBedRoom = rdo.ListTreatmentBedRooms.Where(o => o.TREATMENT_ID == treatmentId).ToList();

                    dataResult = (TreatmentBedRoom != null && TreatmentBedRoom.Count > 0) ? TreatmentBedRoom[0].BED_ROOM_NAME + (!String.IsNullOrEmpty(TreatmentBedRoom[0].BED_NAME) ? " - " + TreatmentBedRoom[0].BED_NAME : "") : "";
                }
                dicTreatmentBedRoomNow.Add(treatmentId, dataResult);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dataResult;
        }

        private void SetSingleKey()
        {
            try
            {
                AddObjectKeyIntoListkey(rdo.rationSum, false);
                AddObjectKeyIntoListkey(rdo.ListSereServ, false);
                AddObjectKeyIntoListkey(rdo.ado, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private class FlFuncElementFunction : TFlexCelUserFunction
        {
            object result = null;
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length < 2)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                try
                {
                    //string KeyGet = Convert.ToString(parameters[1]);
                    string KeyGet = "";
                    if (!String.IsNullOrEmpty(parameters[1].ToString()))
                    {
                        KeyGet = parameters[1].ToString().Replace("\"", string.Empty).Trim();
                    }

                    if (parameters[0] is Dictionary<string, int>)
                    {
                        Dictionary<string, int> DicGet = parameters[0] as Dictionary<string, int>;
                        if (String.IsNullOrEmpty(KeyGet)) return DicGet.Values.Sum();
                        if (!DicGet.ContainsKey(KeyGet))
                        {
                            return null;//
                        }
                        result = DicGet[KeyGet];
                    }
                    else if (parameters[0] is Dictionary<string, long>)
                    {
                        Dictionary<string, long> DicGet = parameters[0] as Dictionary<string, long>;
                        if (String.IsNullOrEmpty(KeyGet)) return DicGet.Values.Sum();
                        if (!DicGet.ContainsKey(KeyGet))
                        {
                            return null;
                        }
                        result = DicGet[KeyGet];
                    }
                    else if (parameters[0] is Dictionary<string, decimal>)
                    {
                        Dictionary<string, decimal> DicGet = parameters[0] as Dictionary<string, decimal>;
                        if (String.IsNullOrEmpty(KeyGet)) return DicGet.Values.Sum();
                        if (!DicGet.ContainsKey(KeyGet))
                        {
                            return null;
                        }
                        result = DicGet[KeyGet];
                    }
                    else if (parameters[0] is Dictionary<string, string>)
                    {
                        Dictionary<string, string> DicGet = parameters[0] as Dictionary<string, string>;
                        if (String.IsNullOrEmpty(KeyGet)) return null;
                        if (!DicGet.ContainsKey(KeyGet))
                        {
                            return null;
                        }
                        result = DicGet[KeyGet];
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
}
