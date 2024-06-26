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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintAggrExpMest
{
    class PrintMps000434
    {
        public PrintMps000434(string printTypeCode, string fileName, ref bool result, List<V_HIS_EXP_MEST> _listExpMestPrint, List<V_HIS_EXP_MEST_MATERIAL> _listExpMestMaterial, List<V_HIS_EXP_MEST_MEDICINE> _listExpMestMedicine, long? roomId, bool printNow)
        {
            try
            {
                Dictionary<long, List<V_HIS_EXP_MEST_MATERIAL>> dicMaterial = new Dictionary<long, List<V_HIS_EXP_MEST_MATERIAL>>();
                Dictionary<long, List<V_HIS_EXP_MEST_MEDICINE>> dicMedicine = new Dictionary<long, List<V_HIS_EXP_MEST_MEDICINE>>();

                List<long> expMestIds = new List<long>();
                if (_listExpMestMaterial != null && _listExpMestMaterial.Count > 0)
                {
                    var expMestMate = _listExpMestMaterial.Where(o => o.IS_NOT_PRES == 1).ToList();
                    foreach (var item in expMestMate)
                    {
                        expMestIds.Add(item.EXP_MEST_ID ?? 0);
                        if (!dicMaterial.ContainsKey(item.EXP_MEST_ID ?? 0))
                            dicMaterial[item.EXP_MEST_ID ?? 0] = new List<V_HIS_EXP_MEST_MATERIAL>();

                        dicMaterial[item.EXP_MEST_ID ?? 0].Add(item);
                    }
                }

                if (_listExpMestMedicine != null && _listExpMestMedicine.Count > 0)
                {
                    var ExpMestMedicine = _listExpMestMedicine.Where(o => o.IS_NOT_PRES == 1).ToList();
                    foreach (var item in ExpMestMedicine)
                    {
                        expMestIds.Add(item.EXP_MEST_ID ?? 0);
                        if (!dicMedicine.ContainsKey(item.EXP_MEST_ID ?? 0))
                            dicMedicine[item.EXP_MEST_ID ?? 0] = new List<V_HIS_EXP_MEST_MEDICINE>();

                        dicMedicine[item.EXP_MEST_ID ?? 0].Add(item);
                    }
                }

                expMestIds = expMestIds.Distinct().ToList();

                //không có chi tiết
                if (expMestIds.Count <= 0)
                {
                    return;
                }

                var ListExpMestPrint = _listExpMestPrint.Where(o => expMestIds.Contains(o.ID)).ToList();
                //không có phiếu xuất
                if (ListExpMestPrint.Count <= 0)
                {
                    return;
                }

                var listTreatment = new List<V_HIS_TREATMENT>();
                List<long> treatmentIds = ListExpMestPrint.Select(s => s.TDL_TREATMENT_ID ?? 0).Distinct().ToList();
                int skip = 0;
                while (treatmentIds.Count - skip > 0)
                {
                    var listIds = treatmentIds.Skip(skip).Take(Base.ConfigData.MaxReqParam).ToList();
                    skip += Base.ConfigData.MaxReqParam;
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentViewFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                    hisTreatmentFilter.IDs = listIds;
                    var apiResult = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisTreatmentFilter, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        listTreatment.AddRange(apiResult);
                    }
                }

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                foreach (var treatment in listTreatment)
                {
                    var listExtMest = ListExpMestPrint.Where(o => o.TDL_TREATMENT_ID == treatment.ID).ToList();

                    List<V_HIS_EXP_MEST_MATERIAL> _listExpMestMaterialPrint = new List<V_HIS_EXP_MEST_MATERIAL>();
                    List<V_HIS_EXP_MEST_MEDICINE> _listExpMestMedicinePrint = new List<V_HIS_EXP_MEST_MEDICINE>();

                    foreach (var exp in listExtMest)
                    {
                        if (dicMaterial.ContainsKey(exp.ID))
                        {
                            _listExpMestMaterialPrint.AddRange(dicMaterial[exp.ID]);
                        }

                        if (dicMedicine.ContainsKey(exp.ID))
                        {
                            _listExpMestMedicinePrint.AddRange(dicMedicine[exp.ID]);
                        }
                    }

                    MPS.Processor.Mps000434.PDO.Mps000434PDO.Mps000434PDOConfig cfg = new MPS.Processor.Mps000434.PDO.Mps000434PDO.Mps000434PDOConfig();
                    cfg.ConfigKeyMergerData = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA");

                    MPS.Processor.Mps000434.PDO.Mps000434PDO mps000434RDO = new MPS.Processor.Mps000434.PDO.Mps000434PDO(
                        treatment,
                        _listExpMestMedicinePrint,
                        _listExpMestMaterialPrint,
                        cfg
                        );

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatment.TREATMENT_CODE, printTypeCode, roomId);

                    if (printNow || HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000434RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000434RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
