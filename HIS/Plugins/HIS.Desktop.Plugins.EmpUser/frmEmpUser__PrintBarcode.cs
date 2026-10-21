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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000521.PDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.EmpUser
{
    /// <summary>
    /// Việc 60557: "In barcode" button on each row prints the staff card (MPS000521) that signs the person in on a
    /// sampling scanner (screen "Kết nối thiết bị lấy mẫu bệnh phẩm"). The column is added in code so that the
    /// shared designer file stays untouched.
    /// </summary>
    public partial class frmEmpUser
    {
        private GridColumn gridColumnPrintBarcode;
        private RepositoryItemButtonEdit btnPrintBarcode;
        private HIS_EMPLOYEE printBarcodeEmployee;

        private void InitPrintBarcodeColumn()
        {
            try
            {
                string caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnPrintBarcode.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                EditorButton button = new EditorButton(ButtonPredefines.Glyph);
                button.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/print/printer_16x16.png");
                button.ToolTip = caption;
                this.btnPrintBarcode = new RepositoryItemButtonEdit();
                this.btnPrintBarcode.AutoHeight = false;
                this.btnPrintBarcode.Buttons.Clear();
                this.btnPrintBarcode.Buttons.Add(button);
                this.btnPrintBarcode.TextEditStyle = TextEditStyles.HideTextEditor;
                this.btnPrintBarcode.ButtonClick += this.btnPrintBarcode_ButtonClick;
                this.gridControl2.RepositoryItems.Add(this.btnPrintBarcode);

                this.gridColumnPrintBarcode = new GridColumn();
                this.gridColumnPrintBarcode.Name = "gridColumnPrintBarcode";
                this.gridColumnPrintBarcode.FieldName = "PRINT_BARCODE";
                this.gridColumnPrintBarcode.Caption = caption;
                this.gridColumnPrintBarcode.ToolTip = caption;
                this.gridColumnPrintBarcode.ColumnEdit = this.btnPrintBarcode;
                this.gridColumnPrintBarcode.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                this.gridColumnPrintBarcode.OptionsColumn.FixedWidth = true;
                this.gridColumnPrintBarcode.OptionsColumn.ShowCaption = false;
                this.gridColumnPrintBarcode.Width = 20;
                this.gridViewFormList.Columns.Add(this.gridColumnPrintBarcode);
                // A visible index set during Load lands one column too early (grid not laid out yet): place it once shown
                this.BeginInvoke(new Action(this.PlacePrintBarcodeColumn));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Right after the row buttons (lock, reset password, roles).
        /// </summary>
        private void PlacePrintBarcodeColumn()
        {
            try
            {
                this.gridColumnPrintBarcode.VisibleIndex =
                    Math.Max(this.gridColumnDEFAULTPASS.VisibleIndex, this.gridColumnROLEUSER.VisibleIndex) + 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrintBarcode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                this.printBarcodeEmployee = this.gridViewFormList.GetFocusedRow() as HIS_EMPLOYEE;
                if (this.printBarcodeEmployee == null || String.IsNullOrWhiteSpace(this.printBarcodeEmployee.LOGINNAME))
                    return;

                Inventec.Common.RichEditor.RichEditorStore richEditorStore = new Inventec.Common.RichEditor.RichEditorStore(
                    HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer,
                    HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR,
                    LanguageManager.GetLanguage(),
                    HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorStore.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__MPS000521, this.DelegateRunPrinterBarcode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegateRunPrinterBarcode(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                HIS_EMPLOYEE employee = this.printBarcodeEmployee;
                if (employee == null)
                    return false;

                HIS_DEPARTMENT department = employee.DEPARTMENT_ID.HasValue
                    ? BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == employee.DEPARTMENT_ID.Value)
                    : null;
                Mps000521ADO card = new Mps000521ADO();
                card.LOGINNAME = employee.LOGINNAME;
                card.USERNAME = employee.TDL_USERNAME;
                card.EMPLOYEE_CODE = employee.EMPLOYEE_CODE;
                card.DEPARTMENT_NAME = department != null ? department.DEPARTMENT_NAME : "";
                card.BARCODE_TEXT = PrintTypeCodeWorker.ACCOUNT_BARCODE_PREFIX + employee.LOGINNAME.Trim();
                Mps000521PDO pdo = new Mps000521PDO(new List<Mps000521ADO> { card });

                string printerName = "";
                Dictionary<string, string> dicPrinter = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicPrinter;
                if (dicPrinter != null && dicPrinter.ContainsKey(printTypeCode))
                    printerName = dicPrinter[printTypeCode];
                MPS.ProcessorBase.PrintConfig.PreviewType previewType =
                    HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2
                    ? MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow
                    : MPS.ProcessorBase.PrintConfig.PreviewType.Show;
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, previewType, printerName));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
