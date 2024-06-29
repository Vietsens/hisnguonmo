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
using DevExpress.XtraTreeList.Nodes;
using MOS.EFMODEL.DataModels;
using Inventec.UC.Paging;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.Filter;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.XtraGrid.Views.Grid;
using AutoMapper;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.MedicalStoreV2.ADO;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.Skins;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using HIS.Desktop.Plugins.MedicalStoreV2.ChooseStore;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.MedicalStoreV2
{
    public partial class UCMedicalStoreV2 : UserControlBase
    {
        MediRecordADO currentMediRecord = null;

        internal void PrintMediRecord(MediRecordADO mediRecordADO)
        {
            try
            {
                this.currentMediRecord = mediRecordADO;
                PrintProcess(PrintTypeMediRecord.IN_BARCODE_MEDI_RECORD_CODE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal enum PrintTypeMediRecord
        {
            IN_BARCODE_MEDI_RECORD_CODE,
        }

        void PrintProcess(PrintTypeMediRecord printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeMediRecord.IN_BARCODE_MEDI_RECORD_CODE:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_BARCODE_MEDI_RECORD_CODE__MPS000094, DelegateRunPrinter);
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
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_BARCODE_MEDI_RECORD_CODE__MPS000094:
                        LoadBieuMauInBarCode(printTypeCode, fileName, ref result);
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

        private void LoadBieuMauInBarCode(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                V_HIS_MEDI_RECORD_1 mediRecord = new V_HIS_MEDI_RECORD_1();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MEDI_RECORD_1>(mediRecord, currentMediRecord);
                MPS.Processor.Mps000094.Mps000094PDO mps = new MPS.Processor.Mps000094.Mps000094PDO(mediRecord);

                MPS.ProcessorBase.Core.PrintData printData = null;

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                else
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                WaitingManager.Hide();

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(currentMediRecord != null ? currentMediRecord.TDL_TREATMENT_CODES : "", printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);
                printData.EmrInputADO = inputADO;

                result = MPS.MpsPrinter.Run(printData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
