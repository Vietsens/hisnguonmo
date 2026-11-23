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

        /// <summary>
        /// Medical records waiting to be printed by the batch print buttons.
        /// Only set while the (modal) template chooser runs, cleared right after.
        /// </summary>
        List<MediRecordADO> printMediRecords = null;

        /// <summary>
        /// "In toàn bộ": print the storage barcode of every medical record on the grid
        /// (current page), stored or not.
        /// </summary>
        private void btnPrintAll_Click(object sender, EventArgs e)
        {
            try
            {
                var mediRecords = gridControlMediRecord.DataSource as List<MediRecordADO>;
                if (mediRecords == null || !mediRecords.Any())
                {
                    MessageManager.Show(Resources.ResourceMessage.KhongCoBenhAnDeIn);
                    return;
                }
                PrintMediRecords(mediRecords.ToList());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// "In đã chọn": print the storage barcode of the ticked medical records only.
        /// </summary>
        private void btnPrintSelected_Click(object sender, EventArgs e)
        {
            try
            {
                var mediRecords = gridControlMediRecord.DataSource as List<MediRecordADO>;
                var mediRecordChecks = mediRecords != null ? mediRecords.Where(o => o.CheckStore).ToList() : null;
                if (mediRecordChecks == null || !mediRecordChecks.Any())
                {
                    MessageManager.Show(Resources.ResourceMessage.VuiLongChonBenhAnCanIn);
                    return;
                }
                PrintMediRecords(mediRecordChecks);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintMediRecords(List<MediRecordADO> mediRecords)
        {
            try
            {
                btnPrintAll.Enabled = false;
                btnPrintSelected.Enabled = false;
                this.printMediRecords = mediRecords;

                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                // RunPrintTemplate shows the template chooser modally, so the callback runs before it returns
                richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_BARCODE_MEDI_RECORD_CODE__MPS000094, DelegateRunPrinterMediRecords);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                this.printMediRecords = null;
                btnPrintAll.Enabled = true;
                btnPrintSelected.Enabled = true;
            }
        }

        /// <summary>
        /// Prints one barcode sheet per medical record, in grid order. Print now or preview
        /// follows the application print mode. A failed sheet is logged and the rest go on.
        /// </summary>
        bool DelegateRunPrinterMediRecords(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.printMediRecords == null || !this.printMediRecords.Any())
                    return false;

                bool isPrintNow = ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2;
                MPS.ProcessorBase.PrintConfig.PreviewType previewType = isPrintNow ? MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow : MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog;
                HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor emrGenerateProcessor = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor();
                long roomId = this.currentModule != null ? this.currentModule.RoomId : 0;
                List<string> failStoreCodes = new List<string>();

                if (isPrintNow)
                    WaitingManager.Show();
                foreach (var item in this.printMediRecords)
                {
                    try
                    {
                        MPS.Processor.Mps000094.Mps000094PDO mps = CreateMps000094PDO(item);
                        MPS.ProcessorBase.Core.PrintData printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps, previewType, "");
                        printData.EmrInputADO = emrGenerateProcessor.GenerateInputADOWithPrintTypeCode(item.TDL_TREATMENT_CODES ?? "", printTypeCode, roomId);

                        if (MPS.MpsPrinter.Run(printData))
                            result = true;
                        else
                            failStoreCodes.Add(item.STORE_CODE);
                    }
                    catch (Exception exItem)
                    {
                        failStoreCodes.Add(item.STORE_CODE);
                        Inventec.Common.Logging.LogSystem.Warn(exItem);
                    }
                }
                WaitingManager.Hide();

                if (failStoreCodes.Any())
                    Inventec.Common.Logging.LogSystem.Warn("Batch print MPS000094 not printed for STORE_CODE: " + String.Join(",", failStoreCodes));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Builds the MPS000094 data for a medical record. The current Mps000094 PDO only has the
        /// V_HIS_TREATMENT_9 constructor (the V_HIS_MEDI_RECORD_1 one was removed) and the template
        /// only prints the barcode of STORE_CODE, so the record's storage code is carried on a
        /// treatment object. This constructor exists in both old and new PDO builds.
        /// </summary>
        private MPS.Processor.Mps000094.Mps000094PDO CreateMps000094PDO(MediRecordADO mediRecord)
        {
            V_HIS_TREATMENT_9 treatment = new V_HIS_TREATMENT_9();
            treatment.STORE_CODE = mediRecord != null ? mediRecord.STORE_CODE : null;
            return new MPS.Processor.Mps000094.Mps000094PDO(treatment);
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
                MPS.Processor.Mps000094.Mps000094PDO mps = CreateMps000094PDO(currentMediRecord);

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
