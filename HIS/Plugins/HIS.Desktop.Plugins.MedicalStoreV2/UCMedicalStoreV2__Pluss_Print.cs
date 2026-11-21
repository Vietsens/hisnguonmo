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
using System.IO;
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
        /// Renders one barcode sheet per medical record (grid order), joins them into a single
        /// workbook (one page per record) and sends it as ONE print job, so a print-to-file
        /// printer asks for a file name once instead of once per record. Print now or preview
        /// follows the application print mode. A sheet that fails to render is logged and skipped.
        /// </summary>
        bool DelegateRunPrinterMediRecords(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.printMediRecords == null || !this.printMediRecords.Any())
                    return false;

                WaitingManager.Show();
                List<MemoryStream> sheets = new List<MemoryStream>();
                List<string> failStoreCodes = new List<string>();
                foreach (var item in this.printMediRecords)
                {
                    try
                    {
                        MemoryStream sheet = RenderMps000094Sheet(item, printTypeCode, fileName);
                        if (sheet != null)
                            sheets.Add(sheet);
                        else
                            failStoreCodes.Add(item.STORE_CODE);
                    }
                    catch (Exception exItem)
                    {
                        failStoreCodes.Add(item.STORE_CODE);
                        Inventec.Common.Logging.LogSystem.Warn(exItem);
                    }
                }
                if (failStoreCodes.Any())
                    Inventec.Common.Logging.LogSystem.Warn("Batch print MPS000094 not rendered for STORE_CODE: " + String.Join(",", failStoreCodes));

                MemoryStream merged = sheets.Any() ? MergeSheetsOnePagePerRecord(sheets) : null;
                WaitingManager.Hide();

                if (merged != null)
                    result = PrintMergedSheets(merged, fileName);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Runs the MPS000094 template for one medical record into memory (no printing).
        /// Returns null when nothing was rendered.
        /// </summary>
        private MemoryStream RenderMps000094Sheet(MediRecordADO mediRecord, string printTypeCode, string fileName)
        {
            MPS.ProcessorBase.Core.PrintData printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, CreateMps000094PDO(mediRecord), MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
            printData.saveMemoryStream = new MemoryStream();
            if (!MPS.MpsPrinter.Run(printData) || printData.saveMemoryStream == null || printData.saveMemoryStream.Length == 0)
                return null;
            printData.saveMemoryStream.Position = 0;
            return printData.saveMemoryStream;
        }

        /// <summary>
        /// Joins rendered sheets into the first one: the print area block of every next sheet is
        /// copied (cells, formats and barcode images) below the previous block, with a page break
        /// in between, and the print area is extended to cover all blocks.
        /// </summary>
        private MemoryStream MergeSheetsOnePagePerRecord(List<MemoryStream> sheets)
        {
            sheets[0].Position = 0;
            if (sheets.Count == 1)
                return sheets[0];

            FlexCel.XlsAdapter.XlsFile dest = new FlexCel.XlsAdapter.XlsFile(true);
            dest.Open(sheets[0]);
            int sheetIndex = dest.ActiveSheet;
            string printAreaName = FlexCel.Core.TXlsNamedRange.GetInternalName(FlexCel.Core.InternalNameRange.Print_Area);
            int printAreaIndex = dest.FindNamedRange(printAreaName, sheetIndex);
            FlexCel.Core.TXlsNamedRange printArea = printAreaIndex > 0 ? dest.GetNamedRange(printAreaIndex) : null;

            int top = printArea != null ? printArea.Top : 1;
            int left = printArea != null ? printArea.Left : 1;
            int bottom = printArea != null ? printArea.Bottom : dest.RowCount;
            int right = printArea != null ? printArea.Right : GetUsedColCount(dest);
            int blockRows = bottom - top + 1;
            FlexCel.Core.TXlsCellRange block = new FlexCel.Core.TXlsCellRange(top, left, bottom, right);

            for (int i = 1; i < sheets.Count; i++)
            {
                FlexCel.XlsAdapter.XlsFile src = new FlexCel.XlsAdapter.XlsFile(true);
                sheets[i].Position = 0;
                src.Open(sheets[i]);
                int destRow = top + blockRows * i;
                dest.InsertAndCopyRange(block, destRow, left, 1, FlexCel.Core.TFlxInsertMode.ShiftRowDown, FlexCel.Core.TRangeCopyMode.All, src, sheetIndex);
                dest.InsertHPageBreak(destRow - 1);
            }

            int lastRow = top + blockRows * sheets.Count - 1;
            if (printArea != null)
                dest.SetNamedRange(new FlexCel.Core.TXlsNamedRange(printArea.Name, sheetIndex, sheetIndex, top, left, lastRow, right, printArea.OptionFlags));
            else
                dest.SetNamedRange(new FlexCel.Core.TXlsNamedRange(printAreaName, sheetIndex, sheetIndex, top, left, lastRow, right, 32));

            MemoryStream result = new MemoryStream();
            dest.Save(result);
            result.Position = 0;
            return result;
        }

        private int GetUsedColCount(FlexCel.XlsAdapter.XlsFile xls)
        {
            int maxCol = 1;
            for (int row = 1; row <= xls.RowCount; row++)
                maxCol = Math.Max(maxCol, xls.ColCountInRow(row) > 0 ? xls.ColFromIndex(row, xls.ColCountInRow(row)) : 1);
            return maxCol;
        }

        /// <summary>
        /// Prints (or previews, per the application print mode) the joined workbook as one job,
        /// with the same options MPS uses for a single sheet.
        /// </summary>
        private bool PrintMergedSheets(MemoryStream merged, string fileName)
        {
            bool isPrintNow = ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2;
            bool isAllowExport = MPS.ProcessorBase.PrintConfig.IsExportExcel.HasValue ? MPS.ProcessorBase.PrintConfig.IsExportExcel.Value : true;
            bool isAllowEditTemplateFile = MPS.ProcessorBase.PrintConfig.IsAllowEditTemplateFile.HasValue ? MPS.ProcessorBase.PrintConfig.IsAllowEditTemplateFile.Value : true;

            Inventec.Common.Print.FlexCelPrintProcessor printProcess = new Inventec.Common.Print.FlexCelPrintProcessor(merged, "", fileName, 1, false, isAllowExport, null, null, null, null, null, null, isAllowEditTemplateFile, false, isPrintNow);
            if (isPrintNow)
                printProcess.Print();
            else
                printProcess.PrintPreview();
            return true;
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
