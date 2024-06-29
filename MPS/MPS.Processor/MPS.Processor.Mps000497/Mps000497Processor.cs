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
using MPS.Processor.Mps000497.ADO;
using MPS.Processor.Mps000497.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000497
{
    public class Mps000497Processor : AbstractProcessor
    {
        Mps000497PDO rdo;

        List<HIS_RATION_GROUP> RationGroups = new List<HIS_RATION_GROUP>();
        List<DepartmentADO> Departments = new List<DepartmentADO>();
        List<V_HIS_SERE_SERV_RATION> ServiceRations = new List<V_HIS_SERE_SERV_RATION>();

        public Mps000497Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000497PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                ProcessListData();
                singleTag.ProcessData(store, singleValueDictionary);

                objectTag.AddObjectData(store, "RationGroups", RationGroups.OrderBy(o => o.RATION_GROUP_NAME).ToList());
                objectTag.AddObjectData(store, "Departments", Departments);
                objectTag.AddObjectData(store, "ServiceRations", ServiceRations.OrderBy(o => o.SERVICE_NAME).ToList());
                objectTag.AddRelationship(store, "RationGroups", "ServiceRations", "ID", "RATION_GROUP_ID");
                objectTag.SetUserFunction(store, "FunDiction", new FunDiction());
                objectTag.SetUserFunction(store, "FunExistsCode", new FunExists());
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
                if (rdo != null)
                {
                    if (rdo._hisRationSums != null && rdo._hisRationSums.Count > 0)
                    {
                        SetSingleKey(new KeyValue("MAX_INTRUCTION_DATE_FROM", rdo._hisRationSums.Min(o => o.MAX_INTRUCTION_DATE)));
                        SetSingleKey(new KeyValue("MAX_INTRUCTION_DATE_TO", rdo._hisRationSums.Max(o => o.MAX_INTRUCTION_DATE)));
                    }

                    if (rdo._hisSereServRation != null && rdo._hisSereServRation.Count > 0)
                    {
                        SetSingleKey(new KeyValue("RATION_TIME_NAME", rdo._hisSereServRation.First().RATION_TIME_NAME));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessListData()
        {
            try
            {
                if (rdo != null && rdo._hisSereServRation != null && rdo._hisSereServRation.Count > 0)
                {
                    rdo._hisSereServRation.ForEach(o => o.RATION_GROUP_ID = o.RATION_GROUP_ID ?? -1);

                    var groupdepa = rdo._hisSereServRation.GroupBy(o => o.REQUEST_DEPARTMENT_ID).ToList();
                    foreach (var department in groupdepa)
                    {
                        DepartmentADO depa = new DepartmentADO();
                        if (rdo._hisDepartment != null)
                        {
                            var de = rdo._hisDepartment.FirstOrDefault(o => o.ID == department.Key);
                            if (de != null)
                            {
                                Inventec.Common.Mapper.DataObjectMapper.Map<DepartmentADO>(depa, de);
                            }
                        }

                        foreach (var item in department)
                        {
                            if (!this.RationGroups.Exists(e => e.ID == (item.RATION_GROUP_ID ?? -1)))
                            {
                                if (rdo._hisRationGroup != null && rdo._hisRationGroup.Count > 0)
                                {
                                    HIS_RATION_GROUP gr = rdo._hisRationGroup.FirstOrDefault(o => o.ID == item.RATION_GROUP_ID);
                                    if (gr == null)
                                    {
                                        gr = new HIS_RATION_GROUP();
                                        gr.ID = -1;
                                        gr.RATION_GROUP_CODE = "KH";
                                        gr.RATION_GROUP_NAME = "Khác";
                                    }

                                    this.RationGroups.Add(gr);
                                }
                            }

                            if (!this.ServiceRations.Exists(o => o.SERVICE_CODE == item.SERVICE_CODE))
                            {
                                this.ServiceRations.Add(item);
                            }
                            if (depa.DIC_AMOUNT.ContainsKey(item.SERVICE_CODE))
                            {
                                depa.DIC_AMOUNT[item.SERVICE_CODE] += item.AMOUNT;
                            }
                            else
                            {
                                depa.DIC_AMOUNT.Add(item.SERVICE_CODE, item.AMOUNT);
                            }
                        }

                        depa.TOTAL_AMOUNT = department.Sum(s => s.AMOUNT);
                        depa.INSTRUCTION_NOTES = string.Join(";", department.Select(o => o.INSTRUCTION_NOTE).Distinct());
                        Departments.Add(depa);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public class FunDiction : TFlexCelUserFunction
        {
            public override object Evaluate(object[] parameters)
            {
                object result = null;
                if (parameters == null || parameters.Length < 2)
                {
                    Inventec.Common.Logging.LogSystem.Error("Bad parameter count in call to Orders() user-defined function");
                    return null;
                }

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

        public class FunExists : TFlexCelUserFunction
        {
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length < 2)
                {
                    Inventec.Common.Logging.LogSystem.Error("Bad parameter count in call to Orders() user-defined function");
                    return null;
                }

                string[] list = parameters[0].ToString().Split(',', ';', '.', ':', '|');
                if (list.Contains(parameters[1].ToString()))
                {
                    return "1";
                }

                return null;
            }
        }
    }
}
