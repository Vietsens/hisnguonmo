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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Print;

namespace HIS.Desktop.Plugins.MediStockSummaryByExpireDate
{
    public partial class UCMediStockSummaryByExpireDate : UserControl
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


                MPS.Core.Mps000131.Mps000131RDO mps000131RDO = new MPS.Core.Mps000131.Mps000131RDO(
                    this.currentMediStock,
                    lstMediInStocks,
                    lstMateInStocks,
                    lstBlood,
                     chkMedicine.Checked,
                     chkMaterial.Checked,
                     chkBlood.Checked
               );

                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.Printer.Run(printTypeCode, fileName, mps000131RDO, MPS.Printer.PreviewType.PrintNow);
                }
                else
                {
                    result = MPS.Printer.Run(printTypeCode, fileName, mps000131RDO, MPS.Printer.PreviewType.ShowDialog);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
