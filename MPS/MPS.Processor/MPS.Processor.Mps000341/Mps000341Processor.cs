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
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using LIS.EFMODEL.DataModels;
using MPS.Processor.Mps000341.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000341
{
    public class Mps000341Processor : AbstractProcessor
    {
        Mps000341PDO rdo;
        List<V_LIS_RESULT> resultPrint1s = new List<V_LIS_RESULT>();
        List<V_LIS_RESULT> resultPrint2s = new List<V_LIS_RESULT>();
        List<V_LIS_RESULT> _listBacteriumCode = new List<V_LIS_RESULT>();
        List<V_LIS_RESULT> _listAntibioticCode = new List<V_LIS_RESULT>();
        public Mps000341Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000341PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetSingleKey();
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "Result1", this.resultPrint1s);
                objectTag.AddObjectData(store, "Result2", this.resultPrint2s);
                objectTag.AddObjectData(store, "Results", this.rdo._Results);
                objectTag.AddObjectData(store, "ListBacteriumCode", this._listBacteriumCode);
                objectTag.AddObjectData(store, "ListAntibioticCode", this._listAntibioticCode);
                objectTag.AddRelationship(store, "ListBacteriumCode", "ListAntibioticCode", "BACTERIUM_CODE", "BACTERIUM_CODE");

                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {

                if (rdo._Sample != null)
                {
                    AddObjectKeyIntoListkey<V_LIS_SAMPLE>(rdo._Sample);
                }
                if (rdo._Sample != null && rdo._PatientConditions != null && rdo._PatientConditions.Count() > 0)
                {
                    var patientCondition = rdo._PatientConditions.Where(o => o.ID == rdo._Sample.PATIENT_CONDITION_ID).FirstOrDefault();
                    var patientConditionCodeStr = "";
                    var patientConditionNameStr = "";
                    if (patientCondition != null)
                    {
                        patientConditionCodeStr = patientCondition.PATIENT_CONDITION_CODE;
                        patientConditionNameStr = patientCondition.PATIENT_CONDITION_NAME;
                    }
                    SetSingleKey(new KeyValue(Mps000341ExtendSingleKey.PATIENT_CONDITION_CODE, patientConditionCodeStr));
                    SetSingleKey(new KeyValue(Mps000341ExtendSingleKey.PATIENT_CONDITION_NAME, patientConditionNameStr));
                }
                if (rdo._SampleService != null)
                {
                    if (!string.IsNullOrWhiteSpace(rdo._SampleService.MICROBIOLOGICAL_RESULT))
                    {
                        SetSingleKey(new KeyValue(Mps000341ExtendSingleKey.CULTURE_RESULT, rdo._SampleService.MICROBIOLOGICAL_RESULT.ToUpper()));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000341ExtendSingleKey.CULTURE_RESULT, "KHÔNG XÁC ĐỊNH"));
                    }

                    if (rdo._Results != null && rdo._Results.Count > 0)
                    {
                        var groups = rdo._Results.GroupBy(o => o.BACTERIUM_CODE).ToList();
                        foreach (var gr in groups)
                        {
                            _listBacteriumCode.Add(gr.FirstOrDefault());
                            foreach (var item in gr)
                            {
                                if (item.ANTIBIOTIC_CODE != null)
                                    _listAntibioticCode.Add(item);
                            }
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _listBacteriumCode), _listBacteriumCode));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _listAntibioticCode), _listAntibioticCode));
                        var first = rdo._Results.FirstOrDefault();
                        SetSingleKey(new KeyValue(Mps000341ExtendSingleKey.BACTERIUM_CODE, first != null ? first.BACTERIUM_CODE : ""));
                        SetSingleKey(new KeyValue(Mps000341ExtendSingleKey.BACTERIUM_NAME, first != null ? first.BACTERIUM_NAME : ""));
                        SetSingleKey(new KeyValue(Mps000341ExtendSingleKey.BACTERIUM_FAMILY_CODE, first != null ? first.BACTERIUM_FAMILY_CODE : ""));
                        SetSingleKey(new KeyValue(Mps000341ExtendSingleKey.BACTERIUM_FAMILY_NAME, first != null ? first.BACTERIUM_FAMILY_NAME : ""));
                        SetSingleKey(new KeyValue(Mps000341ExtendSingleKey.DESCRIPTION, first != null ? first.DESCRIPTION : ""));
                        SetSingleKey(new KeyValue(Mps000341ExtendSingleKey.TECHNIQUE_CODE, first != null ? first.TECHNIQUE_CODE : ""));
                        SetSingleKey(new KeyValue(Mps000341ExtendSingleKey.TECHNIQUE_NAME, first != null ? first.TECHNIQUE_NAME : ""));
                        int count = rdo._Results.Count;
                        int nguyen = count / 2;
                        int du = count % 2;

                        resultPrint1s = rdo._Results.Skip(0).Take(nguyen + (du > 0 ? 1 : 0)).ToList();
                        resultPrint2s = rdo._Results.Skip(nguyen + (du > 0 ? 1 : 0)).ToList();
                        if (du > 0)
                        {
                            resultPrint2s.Add(new V_LIS_RESULT());
                        }
                    }
                    AddObjectKeyIntoListkey<LIS_SAMPLE_SERVICE>(rdo._SampleService, false);

                }
                if (rdo._Machine != null)
                {
                    AddObjectKeyIntoListkey<LIS_MACHINE>(rdo._Machine, false);
                }
                if (rdo._serviceReq != null)
                {
                    AddObjectKeyIntoListkey(rdo._serviceReq, false);
                }
                if (rdo._treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000341ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo._treatment.TDL_PATIENT_DOB)));
                    AddObjectKeyIntoListkey(rdo._treatment, false);
                }
                if (rdo._patient != null)
                {
                    AddObjectKeyIntoListkey(rdo._patient, false);
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultPrint1s), resultPrint1s));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultPrint2s), resultPrint2s));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo._Sample != null && rdo._serviceReq != null && rdo._treatment != null)
                {
                    result = String.Format("{0} {1} {2} {3}", this.printTypeCode, string.Format("TREATMENT_CODE:{0}", rdo._treatment.TREATMENT_CODE), string.Format("SERVICE_REQ_CODE:{0}", rdo._serviceReq.SERVICE_REQ_CODE), rdo._Sample.BARCODE);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
