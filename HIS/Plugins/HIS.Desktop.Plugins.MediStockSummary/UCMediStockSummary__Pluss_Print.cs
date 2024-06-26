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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using MOS.EFMODEL.DataModels;
using System;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediStockSummary
{
    public partial class UCMediStockSummary : HIS.Desktop.Utility.UserControlBase
    {
        internal enum PrintType
        {
            IN_PHIEU_TONG_HOP_TON_KHO_T_VT_M,
        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.IN_PHIEU_TONG_HOP_TON_KHO_T_VT_M:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PHIEU_TONG_HOP_TON_KHO_THUOC_VT_MAU__MPS000131, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

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
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PHIEU_TONG_HOP_TON_KHO_THUOC_VT_MAU__MPS000131:
                        LoadDataPrint(printTypeCode, fileName, ref result);
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

        private void LoadDataPrint(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                V_HIS_MEDI_STOCK _MediStock = new V_HIS_MEDI_STOCK();
                if (this.mediStockIds != null && this.mediStockIds.Count > 0)
                {
                    var dataMedis = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.mediStockIds.Contains(p.ID)).ToList();
                    if (dataMedis != null && dataMedis.Count > 0)
                    {
                        //Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MEDI_STOCK>(this.currentMediStock, dataMedis[0]);
                        // this.currentMediStock.MEDI_STOCK_NAME = "";
                        // this.currentMediStock.DEPARTMENT_NAME = "";
                        int d = 0;
                        foreach (var item in dataMedis)
                        {
                            d++;
                            if (d == 1)
                            {
                                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MEDI_STOCK>(_MediStock, item);
                                _MediStock.MEDI_STOCK_NAME += "; ";
                                _MediStock.DEPARTMENT_NAME += "; ";
                                continue;
                            }
                            _MediStock.MEDI_STOCK_NAME += item.MEDI_STOCK_NAME + "; ";
                            _MediStock.DEPARTMENT_NAME += item.DEPARTMENT_NAME + "; ";
                        }
                    }
                }

                MPS.Processor.Mps000131.PDO.Mps000131PDO mps000131RDO = new MPS.Processor.Mps000131.PDO.Mps000131PDO(
               _MediStock,
                    lstMediInStocks,
                    lstMateInStocks,
                    lstBlood,
                     chkMedicine.Checked,
                     chkMaterial.Checked,
                     chkBlood.Checked
               );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000131RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000131RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, RoomId);
                PrintData.EmrInputADO = inputADO;
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
