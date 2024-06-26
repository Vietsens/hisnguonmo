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
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000360.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000360
{
    public class Mps000360Processor : AbstractProcessor
    {
        Mps000360PDO rdo;
        List<V_HIS_SERE_SERV_TEIN> resultPrint1s = new List<V_HIS_SERE_SERV_TEIN>();
        List<V_HIS_SERE_SERV_TEIN> resultPrint2s = new List<V_HIS_SERE_SERV_TEIN>();
        public Mps000360Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000360PDO)rdoBase;
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

                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "Result1", this.resultPrint1s);
                objectTag.AddObjectData(store, "Result2", this.resultPrint2s);
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
                if (rdo._sereServExt != null)
                {
                    SetSingleKey(new KeyValue(Mps000360ExtendSingleKey.IMPLANTION_RESULT, rdo._sereServExt.IMPLANTION_RESULT));
                    SetSingleKey(new KeyValue(Mps000360ExtendSingleKey.MICROCOPY_RESULT, rdo._sereServExt.MICROCOPY_RESULT));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000360ExtendSingleKey.IMPLANTION_RESULT, ""));
                    SetSingleKey(new KeyValue(Mps000360ExtendSingleKey.MICROCOPY_RESULT, ""));
                }
                if (rdo._SampleService != null)
                {
                    if (rdo._SampleService.BACTERIAL_CULTIVATION_RESULT == 1)
                    {
                        SetSingleKey(new KeyValue(Mps000360ExtendSingleKey.CULTURE_RESULT, "DƯƠNG TÍNH"));
                    }
                    else if (rdo._SampleService.BACTERIAL_CULTIVATION_RESULT == 0)
                    {
                        SetSingleKey(new KeyValue(Mps000360ExtendSingleKey.CULTURE_RESULT, "ÂM TÍNH"));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000360ExtendSingleKey.CULTURE_RESULT, "KHÔNG XÁC ĐỊNH"));
                    }

                    AddObjectKeyIntoListkey<V_LIS_SAMPLE_SERVICE>(rdo._SampleService, false);
                }

                if (rdo._sereServTeins != null && rdo._sereServTeins.Count > 0)
                {
                    rdo._sereServTeins = rdo._sereServTeins.Where(o => !String.IsNullOrWhiteSpace(o.BACTERIUM_CODE) && !String.IsNullOrWhiteSpace(o.ANTIBIOTIC_RESISTANCE_CODE)).OrderBy(o => o.ANTIBIOTIC_RESISTANCE_CODE).ToList();
                    var first = rdo._sereServTeins.FirstOrDefault();
                    SetSingleKey(new KeyValue(Mps000360ExtendSingleKey.BACTERIUM_CODE, first != null ? first.BACTERIUM_CODE : ""));
                    SetSingleKey(new KeyValue(Mps000360ExtendSingleKey.BACTERIUM_NAME, first != null ? first.BACTERIUM_NAME : ""));
                    SetSingleKey(new KeyValue(Mps000360ExtendSingleKey.BACTERIUM_AMOUNT, first != null ? first.BACTERIUM_AMOUNT : ""));
                    SetSingleKey(new KeyValue(Mps000360ExtendSingleKey.BACTERIUM_DENSITY, first != null ? first.BACTERIUM_DENSITY : ""));
                    int count = rdo._sereServTeins.Count;
                    int nguyen = count / 2;
                    int du = count % 2;

                    resultPrint1s = rdo._sereServTeins.Skip(0).Take(nguyen + (du > 0 ? 1 : 0)).ToList();
                    resultPrint2s = rdo._sereServTeins.Skip(nguyen + (du > 0 ? 1 : 0)).ToList();
                    if (du < 0)
                    {
                        resultPrint2s.Add(new V_HIS_SERE_SERV_TEIN());
                    }
                }

                if (rdo._Machine != null)
                {
                    AddObjectKeyIntoListkey<LIS_MACHINE>(rdo._Machine, false);
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultPrint1s), resultPrint1s));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultPrint2s), resultPrint2s));
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
                if (rdo != null && rdo._sereServExt != null)
                {
                    log = "ID điều trị: " + rdo._sereServExt.TDL_TREATMENT_ID;
                    log += " , ID yêu cầu: " + rdo._sereServExt.TDL_SERVICE_REQ_ID;
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
                if (rdo != null && rdo._sereServTeins != null && rdo._sereServTeins.Count > 0)
                {
                    string sereServId = rdo._sereServTeins.OrderBy(o => o.SERE_SERV_ID).First().SERE_SERV_ID + "";
                    string bacteriumCode = "BACTERIUM_CODE:" + rdo._sereServTeins.First().BACTERIUM_CODE;
                    string bacteriumAmount = "BACTERIUM_AMOUNT:" + rdo._sereServTeins.First().BACTERIUM_AMOUNT;
                    string bacteriumDensity = "BACTERIUM_DENSITY:" + rdo._sereServTeins.First().BACTERIUM_DENSITY;
                    string count = rdo._sereServTeins.Count + "";

                    result = String.Format("{0} {1} {2} {3} {4}", sereServId, printTypeCode, bacteriumAmount, bacteriumDensity, count);
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
