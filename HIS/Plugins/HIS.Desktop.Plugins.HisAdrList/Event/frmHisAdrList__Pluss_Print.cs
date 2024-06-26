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
using AutoMapper;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisAdrList.Event;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisAdrList.Run
{
    public partial class frmHisAdrList : HIS.Desktop.Utility.FormBase
    {
        void PrintProcess()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate("Mps000248", DelegateRunPrinter);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000248":
                        Mps000248(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void Mps000248(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HIS_TREATMENT _TreatmentPrint = new HIS_TREATMENT();

                MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                treatmentFilter.ID = this.treatmentId;

                var dataTreaments = new BackendAdapter
                    (param).Get<List<HIS_TREATMENT>>
                    (HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param);
                if (dataTreaments != null && dataTreaments.Count > 0)
                {
                    _TreatmentPrint = dataTreaments[0];
                }

                List<V_HIS_ADR_MEDICINE_TYPE> _AdrMedicineTypeNNs = new List<V_HIS_ADR_MEDICINE_TYPE>();
                List<V_HIS_ADR_MEDICINE_TYPE> _AdrMedicineTypes = new List<V_HIS_ADR_MEDICINE_TYPE>();


                MOS.Filter.HisAdrMedicineTypeViewFilter filter = new MOS.Filter.HisAdrMedicineTypeViewFilter();
                if (this._RowDataPrint != null && this._RowDataPrint.ID > 0)
                    filter.ADR_ID = this._RowDataPrint.ID;
                else
                    return;
                var dataAdrMedicineTypes = new BackendAdapter
                    (param).Get<List<V_HIS_ADR_MEDICINE_TYPE>>
                    ("api/HisAdrMedicineType/GetView", ApiConsumers.MosConsumer, filter, param);
                if (dataAdrMedicineTypes != null && dataAdrMedicineTypes.Count > 0)
                {
                    _AdrMedicineTypeNNs.AddRange(dataAdrMedicineTypes.Where(p => p.IS_ADR.HasValue).ToList());
                    if (_AdrMedicineTypeNNs != null && _AdrMedicineTypeNNs.Count > 0)
                    {
                        _AdrMedicineTypeNNs = _AdrMedicineTypeNNs.OrderBy(p => p.MEDICINE_TYPE_NAME).ToList();

                    }
                    _AdrMedicineTypes.AddRange(dataAdrMedicineTypes.Where(p => p.IS_ADR == null || p.IS_ADR <= 0).ToList());
                    if (_AdrMedicineTypes != null && _AdrMedicineTypes.Count > 0)
                    {
                        _AdrMedicineTypes = _AdrMedicineTypes.OrderBy(p => p.MEDICINE_TYPE_NAME).ToList();
                    }
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_TreatmentPrint != null ? _TreatmentPrint.TREATMENT_CODE : ""), printTypeCode, currentModule.RoomId);

                MPS.Processor.Mps000248.PDO.Mps000248PDO mps000248PDO = new MPS.Processor.Mps000248.PDO.Mps000248PDO
                   (
                   _TreatmentPrint,
                   this._RowDataPrint,
                   _AdrMedicineTypeNNs,
                   _AdrMedicineTypes
                     );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000248PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000248PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
