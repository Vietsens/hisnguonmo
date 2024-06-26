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
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintAggrExpMest
{
    public class PrintAggrExpMestProcessor
    {
        private Inventec.Common.RichEditor.RichEditorStore richEditorMain;
        private bool printNow = false;
        List<V_HIS_EXP_MEST> ListExpMestPrint;
        List<V_HIS_EXP_MEST_MEDICINE> ListExpMestMedicine;
        List<V_HIS_EXP_MEST_MATERIAL> ListExpMestMaterial;
        List<V_HIS_TREATMENT> ListTreatment;
        List<V_HIS_TREATMENT_BED_ROOM> ListTreatmentBedRoom;
        public long? RoomId = null;

        public PrintAggrExpMestProcessor(List<V_HIS_EXP_MEST> _listExpMestPrint)
        {
            this.ListExpMestPrint = _listExpMestPrint;
        }

        public PrintAggrExpMestProcessor(List<V_HIS_EXP_MEST> _listExpMestPrint, List<V_HIS_EXP_MEST_MEDICINE> _listExpMestMedicine, List<V_HIS_EXP_MEST_MATERIAL> _listExpMestMaterial)
        {
            this.ListExpMestPrint = _listExpMestPrint;
            this.ListExpMestMaterial = _listExpMestMaterial;
            this.ListExpMestMedicine = _listExpMestMedicine;
        }

        /// <summary>
        /// in ngay
        /// </summary>
        /// <param name="PrintTypeCode">mã in</param>
        public void Print(string PrintTypeCode)
        {
            try
            {
                Print(PrintTypeCode, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PrintTypeCode">Mã in (44,50,118)</param>
        /// <param name="PrintNow">true/false</param>
        public void Print(string PrintTypeCode, bool PrintNow)
        {
            try
            {
                this.printNow = PrintNow;
                richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);

                richEditorMain.RunPrintTemplate(PrintTypeCode, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                if (ProcessDataForPrint())
                {
                    switch (printCode)
                    {
                        case "Mps000262":
                            new PrintMps000262(printCode, fileName, ref result, ListExpMestPrint, ListExpMestMaterial, ListExpMestMedicine);
                            break;
                        case "Mps000434":
                            new PrintMps000434(printCode, fileName, ref result, ListExpMestPrint, ListExpMestMaterial, ListExpMestMedicine, this.RoomId, this.printNow);
                            break;
                        default:
                            break;
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private bool ProcessDataForPrint()
        {
            bool result = false;
            try
            {
                if (this.ListExpMestPrint != null && this.ListExpMestPrint.Count > 0)
                {
                    ProcessThreadLoadMedimate();

                    if ((this.ListExpMestMedicine == null || this.ListExpMestMedicine.Count <= 0) && (this.ListExpMestMaterial == null || this.ListExpMestMaterial.Count <= 0))
                    {
                        result = false;
                        Inventec.Common.Logging.LogSystem.Error("Khong load duoc chi tiet cua cac phieu " + string.Join(",", ListExpMestPrint.Select(s => s.EXP_MEST_CODE).ToArray()));
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessThreadLoadMedimate()
        {
            System.Threading.Thread medi = new System.Threading.Thread(LoadExpMestMedicie);
            System.Threading.Thread mate = new System.Threading.Thread(LoadExpMestMaterial);
            try
            {
                medi.Start();
                mate.Start();

                medi.Join();
                mate.Join();
            }
            catch (Exception ex)
            {
                mate.Abort();
                medi.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestMaterial()
        {
            try
            {
                if (this.ListExpMestMaterial == null || this.ListExpMestMaterial.Count <= 0)
                {
                    this.ListExpMestMaterial = new List<V_HIS_EXP_MEST_MATERIAL>();
                    List<long> expMestIds = ListExpMestPrint.Select(s => s.ID).Distinct().ToList();
                    var skip = 0;
                    while (expMestIds.Count - skip > 0)
                    {
                        var listIds = expMestIds.Skip(skip).Take(500).ToList();
                        skip += 500;
                        CommonParam param = new CommonParam();
                        HisExpMestMaterialViewFilter hisExpMestMaterialViewFilter = new HisExpMestMaterialViewFilter();
                        hisExpMestMaterialViewFilter.EXP_MEST_IDs = listIds;
                        var apiResult = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisExpMestMaterialViewFilter, param);
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            this.ListExpMestMaterial.AddRange(apiResult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestMedicie()
        {
            try
            {
                if (this.ListExpMestMedicine == null || this.ListExpMestMedicine.Count <= 0)
                {
                    this.ListExpMestMedicine = new List<V_HIS_EXP_MEST_MEDICINE>();
                    List<long> expMestIds = ListExpMestPrint.Select(s => s.ID).Distinct().ToList();
                    var skip = 0;
                    while (expMestIds.Count - skip > 0)
                    {
                        var listIds = expMestIds.Skip(skip).Take(500).ToList();
                        skip += 500;
                        CommonParam param = new CommonParam();
                        HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                        expMestMedicineFilter.EXP_MEST_IDs = listIds;
                        var apiResult = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestMedicineFilter, param);
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            this.ListExpMestMedicine.AddRange(apiResult);
                        }
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
