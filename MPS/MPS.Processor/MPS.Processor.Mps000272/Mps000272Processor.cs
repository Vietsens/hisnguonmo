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
using Inventec.Common.FlexCellExport;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000272.ADO;
using MPS.Processor.Mps000272.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000272
{
    public class Mps000272Processor : AbstractProcessor
    {
        private Mps000272PDO rdo;
        private List<SereServADO> SereServADOs { get; set; }
        private List<PlanTimeADO> PlanTimeADOs { get; set; }

        public Mps000272Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            this.rdo = (Mps000272PDO)this.rdoBase;
        }

        public override bool ProcessData()
        {
            bool flag = false;
            bool result;
            try
            {
                ProcessSingleTag processSingleTag = new ProcessSingleTag();
                ProcessBarCodeTag processBarCodeTag = new ProcessBarCodeTag();
                ProcessObjectTag processObjectTag = new ProcessObjectTag();
                this.store.ReadTemplate(Path.GetFullPath(this.fileName));
                this.DataInputProcess();
                this.PlanDateFromProcess();
                this.ProcessSingleKey();
                this.SetBarcodeKey();
                if (this.PlanTimeADOs == null || this.PlanTimeADOs.Count == 0)
                {
                    result = false;
                    return result;
                }
                processSingleTag.ProcessData(this.store, this.singleValueDictionary);
                processBarCodeTag.ProcessData(this.store, this.dicImage);
                processObjectTag.AddObjectData<PlanTimeADO>(this.store, "PlanTimeADO", this.PlanTimeADOs);

                Inventec.Common.Logging.LogSystem.Info("SereServADOs: "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => SereServADOs), SereServADOs));

                processObjectTag.AddObjectData<SereServADO>(this.store, "SereServADO", this.SereServADOs);
                processObjectTag.AddRelationship(this.store, "PlanTimeADO", "SereServADO", "PLAN_DATE_FROM", "PLAN_DATE_FROM");
                processObjectTag.SetUserFunction(this.store, "ReplaceValue", new Mps000272Processor.ReplaceValueFunction());

                //flexCel.SetUserFunction("FlFuncElement", new RDOElement());mẫu
                processObjectTag.SetUserFunction(this.store, "FlFuncElement", new Mps000272Processor.FlFuncElementFunction());
                
                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
                LogSystem.Error(ex);
            }
            result = flag;
            return result;
        }

        private void DataInputProcess()
        {
            try
            {
                this.SereServADOs = (from r in this.rdo.SereServ13s
                                     select new SereServADO(r, this.rdo.EkipPlanUsers, this.rdo.ExecuteRole, this.rdo.HisExecuteRoles) into o
                                     orderby (o.PLAN_TIME_FROM ?? 99999999999999)
                                     select o).ToList<SereServADO>();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void PlanDateFromProcess()
        {
            try
            {
                this.PlanTimeADOs = new List<PlanTimeADO>();
                if (this.SereServADOs != null && this.SereServADOs.Count > 0)
                {
                    var enumerable = this.SereServADOs.GroupBy(o => o.PLAN_DATE_FROM).ToList();
                    foreach (var current in enumerable)
                    {
                        PlanTimeADO planTimeADO = new PlanTimeADO();
                        SereServADO sereServADO = current.First<SereServADO>();
                        planTimeADO.PLAN_DATE_FROM = sereServADO.PLAN_DATE_FROM;
                        if (planTimeADO.PLAN_DATE_FROM.HasValue)
                        {
                            planTimeADO.DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(planTimeADO.PLAN_DATE_FROM.Value);
                            int num = (int)(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(planTimeADO.PLAN_DATE_FROM.Value).Value.DayOfWeek + 1);
                            if (num == 1)
                            {
                                planTimeADO.INDEX_OF_WEEK = "Chủ nhật";
                            }
                            else
                            {
                                planTimeADO.INDEX_OF_WEEK = "Thứ " + num;
                            }
                        }
                        this.PlanTimeADOs.Add(planTimeADO);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ProcessSingleKey()
        {
            try
            {
                if (this.rdo.PtttCalendar != null)
                {
                    base.SetSingleKey(new KeyValue("TIME_FROM_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo.PtttCalendar.TIME_FROM)));
                    base.SetSingleKey(new KeyValue("TIME_TO_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo.PtttCalendar.TIME_TO)));
                    if (!String.IsNullOrWhiteSpace(this.rdo.PtttCalendar.DEPARTMENT_NAME))
                        this.rdo.PtttCalendar.DEPARTMENT_NAME = this.rdo.PtttCalendar.DEPARTMENT_NAME.ToUpper();
                }

                base.AddObjectKeyIntoListkey<V_HIS_PTTT_CALENDAR>(this.rdo.PtttCalendar);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        internal void SetBarcodeKey()
        {
            try
            {
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private class ReplaceValueFunction : TFlexCelUserFunction
        {
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                {
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
                }
                object result;
                try
                {
                    string text = string.Concat(parameters[0]);
                    if (!string.IsNullOrEmpty(text))
                    {
                        text = text.Replace(';', '/');
                    }
                    result = text;
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                    result = parameters[0];
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
                    string KeyGet ="";
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
