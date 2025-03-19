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
using FlexCel.Core;
using FlexCel.Render;
using FlexCel.XlsAdapter;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;
using DevExpress.XtraGrid.Columns;
using System.Collections.Generic;
using System.Text;
using Inventec.Common.FlexCelPrint.Ado;

namespace Inventec.Common.FlexCelPrint
{
    /// <summary>
    /// Printing / Previewing and Exporting xls files.
    /// </summary>
    public partial class frmSetupPrintPreviewMerge : DevExpress.XtraEditors.XtraForm
    {
        private void LoadSheetConfig()
        {
            try
            {
                ExcelFile Xls = flexCelPrintDocument1.Workbook;

                //Load other settings
                ReadOtherSettings(Xls);
                //Load all available printers
                LoadPrinters(Xls);
                //Load paper sizes
                LoadPaperSizes(Xls);
                //Load paper sources
                LoadPaperSources(Xls);
                //Load files to gridViewPopupMenu
                LoadGridViewPopupMenu();
                

                numericUpDownFromPage.EditValue = null;
                numericUpDownToPage.EditValue = null;

                chkAllPages.Checked = true;
                numericUpDownFromPage.Enabled = numericUpDownToPage.Enabled = false;

                //Landscape or portrait
                rdLandscape.Checked = landscape;
                rdPartrain.Checked = !landscape;

                //center on page
                chkHorizontally.Checked = printHCentered;
                chkVertically.Checked = printVCentered;

                chkBlackAndWhite.Checked = blackAndWhite;

                spinZoom.Value = edZoom;

                isFirstLoadPaged = true;

                if (String.IsNullOrEmpty(this.pathFileTemplate))
                    //layoutControlItem31.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    btnOpenFileTemplate.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadGridViewPopupMenu()
        {
            try
            {
                var data = partialFiles_Excel;
                gridControlPopupMenu.DataSource = partialFiles_Excel;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void LoadPrinters(ExcelFile Xls)
        {
            try
            {
                //Load all avaiable printers
                if (PrinterSettings.InstalledPrinters != null)
                {
                    foreach (String printer in PrinterSettings.InstalledPrinters)
                    {
                        cboPrinters.Properties.Items.Add(printer);
                        if (flexCelPrintDocument1.PrinterSettings.PrinterName == printer)
                        {
                            cboPrinters.EditValue = flexCelPrintDocument1.PrinterSettings.PrinterName;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(defaultPrintName))
                {
                    cboPrinters.EditValue = defaultPrintName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void LoadPaperSizes(ExcelFile Xls)
        {
            try
            {
                Inventec.Common.FlexCelPrint.Loader.PaperSizeLoader.LoadData(flexCelPrintDocument1.PrinterSettings, cboPaperSize);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Xls.PrintPaperDimensions), Xls.PrintPaperDimensions) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Xls.PrintPaperSize), Xls.PrintPaperSize.ToString()));
                //Get all paper sizes and add them to the combo box list
                foreach (PaperSize size in flexCelPrintDocument1.PrinterSettings.PaperSizes)
                {
                    if (size.RawKind.ToString() == Xls.PrintPaperSize.ToString())
                    {
                        cboPaperSize.EditValue = size.RawKind.ToString();
                        currentPaperSize = size;
                        break;
                    }
                    else
                    {
                        if (size.Kind.ToString() == Xls.PrintPaperSize.ToString())
                        {
                            cboPaperSize.EditValue = size.RawKind.ToString();
                            currentPaperSize = size;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void LoadPaperSources(ExcelFile Xls)
        {
            try
            {
                //int counter = 0;
                //PrinterSettings settings = new PrinterSettings();
                //cboSourcePage.DisplayMember = "SourceName";
                //Add all paper sources to the combo box
                foreach (PaperSource source in flexCelPrintDocument1.PrinterSettings.PaperSources)
                {
                    if (Enum.IsDefined(source.Kind.GetType(), source.Kind))
                    {
                        cboSourcePage.Properties.Items.Add(source.SourceName);
                        if (source.SourceName == flexCelPrintDocument1.PrinterSettings.DefaultPageSettings.PaperSource.SourceName)
                        {
                            cboSourcePage.EditValue = source.SourceName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ReadOtherSettings(ExcelFile Xls)
        {
            try
            {
                TXlsMargins m = Xls.GetPrintMargins();

                //Page margins  
                edl = m.Left;
                edt = m.Top;
                edr = m.Right;
                edb = m.Bottom;
                edf = m.Footer;
                edh = m.Header;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => m), m));
                chGridLines = false;
                chHeadings = false;
                chFormulaText = Xls.ShowFormulaText;

                chPrintLeft = (Xls.PrintOptions & TPrintOptions.LeftToRight) != 0;
                edHeader = Xls.PageHeader;
                edFooter = Xls.PageFooter;
                chFitIn = Xls.PrintToFit;
                edHPages = Xls.PrintNumberOfHorizontalPages.ToString();
                edVPages = Xls.PrintNumberOfVerticalPages.ToString();
                //Xls = true;
                //trackBarZoomPage.Value = 100;//100
                edZoom = Xls.PrintScale;
                numericUpDownCopies.Value = Xls.PrintCopies;

                //Landscape or portrait
                landscape = (Xls.PrintOptions & TPrintOptions.Orientation) == 0;

                //center on page
                printHCentered = Xls.PrintHCentered;
                printVCentered = Xls.PrintVCentered;

                //Set other default properties
                //PageSettings pgSettings = flexCelPrintDocument1.DefaultPageSettings;
                //Color printing
                blackAndWhite = !flexCelPrintDocument1.PrinterSettings.DefaultPageSettings.Color;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        bool FoundMatchPageSize()
        {
            bool foundMatchingPaperSize = false;
            try
            {
                for (int index = 0; index < flexCelPrintDocument1.PrinterSettings.PaperSizes.Count; index++)
                {
                    if (flexCelPrintDocument1.PrinterSettings.PaperSizes[index].Height == flexCelPrintDocument1.PrinterSettings.DefaultPageSettings.PaperSize.Height && flexCelPrintDocument1.PrinterSettings.PaperSizes[index].Width == flexCelPrintDocument1.PrinterSettings.DefaultPageSettings.PaperSize.Width)
                    {
                        flexCelPrintDocument1.PrinterSettings.DefaultPageSettings.PaperSize = flexCelPrintDocument1.PrinterSettings.PaperSizes[index];
                        flexCelPrintDocument1.DefaultPageSettings.PaperSize = flexCelPrintDocument1.PrinterSettings.PaperSizes[index];
                        foundMatchingPaperSize = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }

            return foundMatchingPaperSize;
        }

        private void InitPrintPreviewControl()
        {
            try
            {
                if (!HasFileOpen()) return;
                if (!LoadPreferences()) return;

                //TPaperDimensions t = flexCelPrintDocument1.Workbook.PrintPaperDimensions;
                //PaperSize newPsize = new PaperSize(t.PaperName, System.Convert.ToInt32(t.Width), System.Convert.ToInt32(t.Height));
                //newPsize.RawKind = flexCelPrintDocument1.DefaultPageSettings.PaperSize.RawKind;
                //flexCelPrintDocument1.DefaultPageSettings.PaperSize = newPsize;
                //flexCelPrintDocument1.PrinterSettings.DefaultPageSettings.PaperSize = newPsize;
                
                printPreviewControl1.Document = flexCelPrintDocument1;
                printPreviewControl1.StartPage = flexCelPrintDocument1.PrinterSettings.FromPage;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool CheckExistsPageSize(PaperSize requiredPaperSize)
        {
            bool foundExistsPageSize = false;
            for (int index = 0; index < flexCelPrintDocument1.PrinterSettings.PaperSizes.Count; index++)
            {
                if (flexCelPrintDocument1.PrinterSettings.PaperSizes[index].Height == requiredPaperSize.Height && flexCelPrintDocument1.PrinterSettings.PaperSizes[index].Width == requiredPaperSize.Width)
                {
                    flexCelPrintDocument1.PrinterSettings.DefaultPageSettings.PaperSize = flexCelPrintDocument1.PrinterSettings.PaperSizes[index];
                    flexCelPrintDocument1.DefaultPageSettings.PaperSize = flexCelPrintDocument1.PrinterSettings.PaperSizes[index];

                    foundExistsPageSize = true;
                    Inventec.Common.Logging.LogSystem.Debug(String.Format("Paper size for printer {0} = {1}", flexCelPrintDocument1.PrinterSettings.PrinterName, flexCelPrintDocument1.PrinterSettings.PaperSizes[index]));
                    break;
                }
            }
            return foundExistsPageSize;
        }

        private bool HasFileOpen()
        {
            if (flexCelPrintDocument1.Workbook == null && (this.partialFiles_Excel == null || this.partialFiles_Excel.Count == 0))
            {
               MessageBox.Show("You need to open a file first.");
               return false;
            }
            return true;
        }

        private bool LoadPreferences()
        {
            //NOTE: THERE SHOULD BE *A LOT* MORE VALIDATION OF VALUES ON THIS METHOD. (For example, validate that margins are between bounds)
            // As this is a simple demo, they are not included. 
            try
            {
                flexCelPrintDocument1.AllVisibleSheets = false;
                flexCelPrintDocument1.ResetPageNumberOnEachSheet = cbResetPageNumber;
                flexCelPrintDocument1.AntiAliasedText = chAntiAlias;

                //Color printing
                flexCelPrintDocument1.PrinterSettings.DefaultPageSettings.Color = chkBlackAndWhite.Checked;

                ExcelFile Xls = flexCelPrintDocument1.Workbook;
                Xls.PrintGridLines = false;
                Xls.PrintHeadings = false;
                Xls.PageHeader = edHeader;
                Xls.PageFooter = edFooter;
                Xls.ShowFormulaText = chFormulaText;

                // hiển thị gọn trong 1 trang
                //Xls.PrintToFit = true;
                //Xls.PrintNumberOfHorizontalPages = 0;
                //Xls.PrintNumberOfVerticalPages = 0;

                //Set zoom
                try
                {
                    Xls.PrintScale = (int)(spinZoom.Value * (decimal)0.95);// trackBarZoomPage.Value;
                }
                catch
                {
                    MessageBox.Show("Invalid Zoom");
                    return false;
                }

                if (cboPaperSize.EditValue != null)
                {
                    foreach (PaperSize size in flexCelPrintDocument1.PrinterSettings.PaperSizes)
                    {
                        if (size.RawKind.ToString() == cboPaperSize.EditValue.ToString())
                        {
                            flexCelPrintDocument1.DefaultPageSettings.PaperSize = size;
                            flexCelPrintDocument1.PrinterSettings.DefaultPageSettings.PaperSize = size;

                            switch (size.Kind)
                            {
                                #region size
                                case PaperKind.A2:
                                    Xls.PrintPaperSize = TPaperSize.A2;
                                    break;
                                case PaperKind.A3:
                                    Xls.PrintPaperSize = TPaperSize.A3;
                                    break;
                                case PaperKind.A3Extra:
                                    Xls.PrintPaperSize = TPaperSize.A3Extra;
                                    break;
                                case PaperKind.A3ExtraTransverse:
                                    Xls.PrintPaperSize = TPaperSize.A3ExtraTransverse;
                                    break;
                                case PaperKind.A3Rotated:
                                    Xls.PrintPaperSize = TPaperSize.A3Rotated;
                                    break;
                                case PaperKind.A3Transverse:
                                    Xls.PrintPaperSize = TPaperSize.A3Transverse;
                                    break;
                                case PaperKind.A4:
                                    Xls.PrintPaperSize = TPaperSize.A4;
                                    break;
                                case PaperKind.A4Extra:
                                    Xls.PrintPaperSize = TPaperSize.A4Extra;
                                    break;
                                case PaperKind.A4Plus:
                                    Xls.PrintPaperSize = TPaperSize.A4Plus;
                                    break;
                                case PaperKind.A4Rotated:
                                    Xls.PrintPaperSize = TPaperSize.A4Rotated;
                                    break;
                                case PaperKind.A4Small:
                                    Xls.PrintPaperSize = TPaperSize.A4small;
                                    break;
                                case PaperKind.A4Transverse:
                                    Xls.PrintPaperSize = TPaperSize.A4Transverse;
                                    break;
                                case PaperKind.A5:
                                    Xls.PrintPaperSize = TPaperSize.A5;
                                    break;
                                case PaperKind.A5Extra:
                                    Xls.PrintPaperSize = TPaperSize.A5Extra;
                                    break;
                                case PaperKind.A5Rotated:
                                    Xls.PrintPaperSize = TPaperSize.A5Rotated;
                                    break;
                                case PaperKind.A5Transverse:
                                    Xls.PrintPaperSize = TPaperSize.A5Transverse;
                                    break;
                                case PaperKind.A6:
                                    Xls.PrintPaperSize = TPaperSize.A6;
                                    break;
                                case PaperKind.A6Rotated:
                                    Xls.PrintPaperSize = TPaperSize.A6Rotated;
                                    break;
                                case PaperKind.APlus:
                                    Xls.PrintPaperSize = TPaperSize.SuperA_A4;
                                    break;
                                case PaperKind.B4:
                                    Xls.PrintPaperSize = TPaperSize.B4_ISO;
                                    break;
                                case PaperKind.B4Envelope:
                                    Xls.PrintPaperSize = TPaperSize.B4_ISO_2;
                                    break;
                                case PaperKind.B4JisRotated:
                                    Xls.PrintPaperSize = TPaperSize.B4_JIS_Rotated;
                                    break;
                                case PaperKind.B5:
                                    Xls.PrintPaperSize = TPaperSize.B5_ISO;
                                    break;
                                case PaperKind.B5Envelope:
                                    Xls.PrintPaperSize = TPaperSize.B5_JIS;
                                    break;
                                case PaperKind.B5Extra:
                                    Xls.PrintPaperSize = TPaperSize.B5_ISO_Extra;
                                    break;
                                case PaperKind.B5JisRotated:
                                    Xls.PrintPaperSize = TPaperSize.B5_JIS_Rotated;
                                    break;
                                case PaperKind.B5Transverse:
                                    Xls.PrintPaperSize = TPaperSize.B5_JIS_Transverse;
                                    break;
                                case PaperKind.B6Envelope:
                                    Xls.PrintPaperSize = TPaperSize.B6_ISO;
                                    break;
                                case PaperKind.B6Jis:
                                    Xls.PrintPaperSize = TPaperSize.B6_JIS;
                                    break;
                                case PaperKind.B6JisRotated:
                                    Xls.PrintPaperSize = TPaperSize.B6_JIS_Rotated;
                                    break;
                                case PaperKind.BPlus:
                                    Xls.PrintPaperSize = TPaperSize.SuperB_A3;
                                    break;
                                case PaperKind.C3Envelope:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeC3;
                                    break;
                                case PaperKind.C4Envelope:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeC4;
                                    break;
                                case PaperKind.C5Envelope:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeC5;
                                    break;
                                case PaperKind.C65Envelope:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeC6_C5;
                                    break;
                                case PaperKind.C6Envelope:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeC6;
                                    break;
                                case PaperKind.CSheet:
                                    Xls.PrintPaperSize = TPaperSize.C;
                                    break;
                                case PaperKind.Custom:
                                    Xls.PrintPaperSize = TPaperSize.Undefined;
                                    break;
                                case PaperKind.DLEnvelope:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.DSheet:
                                    Xls.PrintPaperSize = TPaperSize.D;
                                    break;
                                case PaperKind.ESheet:
                                    Xls.PrintPaperSize = TPaperSize.E;
                                    break;
                                case PaperKind.Executive:
                                    Xls.PrintPaperSize = TPaperSize.Executive;
                                    break;
                                case PaperKind.Folio:
                                    Xls.PrintPaperSize = TPaperSize.Folio;
                                    break;
                                case PaperKind.GermanLegalFanfold:
                                    Xls.PrintPaperSize = TPaperSize.GermanLegalFanfold;
                                    break;
                                case PaperKind.GermanStandardFanfold:
                                    Xls.PrintPaperSize = TPaperSize.GermanStdFanfold;
                                    break;
                                case PaperKind.InviteEnvelope:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeInvite;
                                    break;
                                case PaperKind.IsoB4:
                                    Xls.PrintPaperSize = TPaperSize.B4_ISO;
                                    break;
                                case PaperKind.ItalyEnvelope:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeItaly;
                                    break;
                                case PaperKind.JapaneseDoublePostcard:
                                    Xls.PrintPaperSize = TPaperSize.JapanesePostcard;
                                    break;
                                case PaperKind.JapaneseDoublePostcardRotated:
                                    Xls.PrintPaperSize = TPaperSize.JapanesePostcardRot;
                                    break;
                                case PaperKind.JapaneseEnvelopeChouNumber3:
                                    Xls.PrintPaperSize = TPaperSize.JapanesePostcard;
                                    break;
                                case PaperKind.JapaneseEnvelopeChouNumber3Rotated:
                                    Xls.PrintPaperSize = TPaperSize.JapanesePostcard;
                                    break;
                                case PaperKind.JapaneseEnvelopeChouNumber4:
                                    Xls.PrintPaperSize = TPaperSize.JapanesePostcard;
                                    break;
                                case PaperKind.JapaneseEnvelopeChouNumber4Rotated:
                                    Xls.PrintPaperSize = TPaperSize.JapanesePostcard;
                                    break;
                                case PaperKind.JapaneseEnvelopeKakuNumber2:
                                    Xls.PrintPaperSize = TPaperSize.JapanesePostcard;
                                    break;
                                case PaperKind.JapaneseEnvelopeKakuNumber2Rotated:
                                    Xls.PrintPaperSize = TPaperSize.JapanesePostcard;
                                    break;
                                case PaperKind.JapaneseEnvelopeKakuNumber3:
                                    Xls.PrintPaperSize = TPaperSize.JapanesePostcard;
                                    break;
                                case PaperKind.JapaneseEnvelopeKakuNumber3Rotated:
                                    Xls.PrintPaperSize = TPaperSize.JapanesePostcard;
                                    break;
                                case PaperKind.JapaneseEnvelopeYouNumber4:
                                    Xls.PrintPaperSize = TPaperSize.JapanesePostcard;
                                    break;
                                case PaperKind.JapaneseEnvelopeYouNumber4Rotated:
                                    Xls.PrintPaperSize = TPaperSize.JapanesePostcard;
                                    break;
                                case PaperKind.JapanesePostcard:
                                    Xls.PrintPaperSize = TPaperSize.JapanesePostcard;
                                    break;
                                case PaperKind.JapanesePostcardRotated:
                                    Xls.PrintPaperSize = TPaperSize.JapanesePostcard;
                                    break;
                                case PaperKind.Ledger:
                                    Xls.PrintPaperSize = TPaperSize.Ledger;
                                    break;
                                case PaperKind.Legal:
                                    Xls.PrintPaperSize = TPaperSize.Legal;
                                    break;
                                case PaperKind.LegalExtra:
                                    Xls.PrintPaperSize = TPaperSize.LegalExtra;
                                    break;
                                case PaperKind.Letter:
                                    Xls.PrintPaperSize = TPaperSize.Letter;
                                    break;
                                case PaperKind.LetterExtra:
                                    Xls.PrintPaperSize = TPaperSize.LetterExtra;
                                    break;
                                case PaperKind.LetterExtraTransverse:
                                    Xls.PrintPaperSize = TPaperSize.LetterExtraTransv;
                                    break;
                                case PaperKind.LetterPlus:
                                    Xls.PrintPaperSize = TPaperSize.LetterPlus;
                                    break;
                                case PaperKind.LetterRotated:
                                    Xls.PrintPaperSize = TPaperSize.LetterRotated;
                                    break;
                                case PaperKind.LetterSmall:
                                    Xls.PrintPaperSize = TPaperSize.Lettersmall;
                                    break;
                                case PaperKind.LetterTransverse:
                                    Xls.PrintPaperSize = TPaperSize.LetterTransverse;
                                    break;
                                case PaperKind.MonarchEnvelope:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeMonarch;
                                    break;
                                case PaperKind.Note:
                                    Xls.PrintPaperSize = TPaperSize.Note;
                                    break;
                                case PaperKind.Number10Envelope:
                                    Xls.PrintPaperSize = TPaperSize.Envelope10;
                                    break;
                                case PaperKind.Number11Envelope:
                                    Xls.PrintPaperSize = TPaperSize.Envelope11;
                                    break;
                                case PaperKind.Number12Envelope:
                                    Xls.PrintPaperSize = TPaperSize.Envelope12;
                                    break;
                                case PaperKind.Number14Envelope:
                                    Xls.PrintPaperSize = TPaperSize.Envelope14;
                                    break;
                                case PaperKind.Number9Envelope:
                                    Xls.PrintPaperSize = TPaperSize.Envelope9;
                                    break;
                                case PaperKind.PersonalEnvelope:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeInvite;
                                    break;
                                case PaperKind.Prc16K:
                                    Xls.PrintPaperSize = TPaperSize.s63_4Envelope;
                                    break;
                                case PaperKind.Prc16KRotated:
                                    Xls.PrintPaperSize = TPaperSize.s63_4Envelope;
                                    break;
                                case PaperKind.Prc32K:
                                    Xls.PrintPaperSize = TPaperSize.s63_4Envelope;
                                    break;
                                case PaperKind.Prc32KBig:
                                    Xls.PrintPaperSize = TPaperSize.s63_4Envelope;
                                    break;
                                case PaperKind.Prc32KBigRotated:
                                    Xls.PrintPaperSize = TPaperSize.s63_4Envelope;
                                    break;
                                case PaperKind.Prc32KRotated:
                                    Xls.PrintPaperSize = TPaperSize.s63_4Envelope;
                                    break;
                                case PaperKind.PrcEnvelopeNumber1:
                                    Xls.PrintPaperSize = TPaperSize.Envelope10;
                                    break;
                                case PaperKind.PrcEnvelopeNumber10:
                                    Xls.PrintPaperSize = TPaperSize.Envelope10;
                                    break;
                                case PaperKind.PrcEnvelopeNumber10Rotated:
                                    Xls.PrintPaperSize = TPaperSize.Envelope10;
                                    break;
                                case PaperKind.PrcEnvelopeNumber1Rotated:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.PrcEnvelopeNumber2:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.PrcEnvelopeNumber2Rotated:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.PrcEnvelopeNumber3:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.PrcEnvelopeNumber3Rotated:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.PrcEnvelopeNumber4:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.PrcEnvelopeNumber4Rotated:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.PrcEnvelopeNumber5:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.PrcEnvelopeNumber5Rotated:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.PrcEnvelopeNumber6:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.PrcEnvelopeNumber6Rotated:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.PrcEnvelopeNumber7:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.PrcEnvelopeNumber7Rotated:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.PrcEnvelopeNumber8:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.PrcEnvelopeNumber8Rotated:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.PrcEnvelopeNumber9:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.PrcEnvelopeNumber9Rotated:
                                    Xls.PrintPaperSize = TPaperSize.EnvelopeDL;
                                    break;
                                case PaperKind.Quarto:
                                    Xls.PrintPaperSize = TPaperSize.Quarto;
                                    break;
                                case PaperKind.Standard10x11:
                                    Xls.PrintPaperSize = TPaperSize.s10x11;
                                    break;
                                case PaperKind.Standard10x14:
                                    Xls.PrintPaperSize = TPaperSize.s10x14;
                                    break;
                                case PaperKind.Standard11x17:
                                    Xls.PrintPaperSize = TPaperSize.s11x17;
                                    break;
                                case PaperKind.Standard12x11:
                                    Xls.PrintPaperSize = TPaperSize.s12x11;
                                    break;
                                case PaperKind.Standard15x11:
                                    Xls.PrintPaperSize = TPaperSize.s15x11;
                                    break;
                                case PaperKind.Standard9x11:
                                    Xls.PrintPaperSize = TPaperSize.s9x11;
                                    break;
                                case PaperKind.Statement:
                                    Xls.PrintPaperSize = TPaperSize.Statement;
                                    break;
                                case PaperKind.Tabloid:
                                    Xls.PrintPaperSize = TPaperSize.Tabloid;
                                    break;
                                case PaperKind.TabloidExtra:
                                    Xls.PrintPaperSize = TPaperSize.TabloidExtra;
                                    break;
                                case PaperKind.USStandardFanfold:
                                    Xls.PrintPaperSize = TPaperSize.USStandardFanfold;
                                    break;
                                default:
                                    break;
                                #endregion
                            }

                            break;
                        }
                    }
                }
                if (cboSourcePage.EditValue != null)
                {
                    foreach (PaperSource source in flexCelPrintDocument1.PrinterSettings.PaperSources)
                    {
                        if (source.SourceName == cboSourcePage.EditValue.ToString())
                        {
                            flexCelPrintDocument1.DefaultPageSettings.PaperSource = source;
                            flexCelPrintDocument1.PrinterSettings.DefaultPageSettings.PaperSource = source;
                            break;
                        }
                    }
                }
                if (cboPrinters.EditValue != null && PrinterSettings.InstalledPrinters != null)
                {
                    foreach (String printer in PrinterSettings.InstalledPrinters)
                    {
                        if (printer == cboPrinters.EditValue.ToString())
                        {
                            flexCelPrintDocument1.PrinterSettings.PrinterName = printer;
                            break;
                        }
                    }
                }

                TXlsMargins m = new TXlsMargins();
                m.Left = edl;
                m.Top = edt;
                m.Right = edr;
                m.Bottom = edb;
                m.Header = edh;
                m.Footer = edf;
                Xls.SetPrintMargins(m);

                flexCelPrintDocument1.DocumentName = flexCelPrintDocument1.Workbook.ActiveFileName + " - Sheet " + flexCelPrintDocument1.Workbook.ActiveSheetByName;
                flexCelPrintDocument1.DefaultPageSettings.Landscape = rdLandscape.Checked;
                flexCelPrintDocument1.PrinterSettings.DefaultPageSettings.Landscape = rdLandscape.Checked;
                flexCelPrintDocument1.PrinterSettings.Copies = (short)numericUpDownCopies.Value;

                flexCelPrintDocument1.Workbook.PrintLandscape = rdLandscape.Checked;
                flexCelPrintDocument1.Workbook.PrintVCentered = chkVertically.Checked;
                flexCelPrintDocument1.Workbook.PrintHCentered = chkHorizontally.Checked;

                //Page range
                if (chkPages.Checked)
                {
                    if (numericUpDownFromPage.EditValue != null && numericUpDownToPage.EditValue != null)
                    {
                        flexCelPrintDocument1.PrinterSettings.PrintRange = PrintRange.SomePages;
                        flexCelPrintDocument1.PrinterSettings.FromPage = (int)numericUpDownFromPage.Value;
                        flexCelPrintDocument1.PrinterSettings.ToPage = (int)numericUpDownToPage.Value;
                        maxPage = (int)numericUpDownToPage.Value;
                        page = (int)numericUpDownFromPage.Value;
                    }
                }
                else if (chkCurrentPage.Checked)
                {
                    flexCelPrintDocument1.PrinterSettings.PrintRange = PrintRange.CurrentPage;
                    //flexCelPrintDocument1.PrinterSettings.FromPage = (int)spinChangePage.Value;
                    //flexCelPrintDocument1.PrinterSettings.ToPage = (int)spinChangePage.Value;
                    //flexCelPrintDocument1.DefaultPageSettings.PrinterSettings.PrintRange = PrintRange.CurrentPage;
                    //maxPage = (int)spinChangePage.Value;
                    //page = (int)spinChangePage.Value;
                }
                else// if (chkAllPages.Checked)
                {
                    flexCelPrintDocument1.PrinterSettings.PrintRange = PrintRange.AllPages;
                    flexCelPrintDocument1.PrinterSettings.FromPage = 0;
                    flexCelPrintDocument1.PrinterSettings.ToPage = 0;
                    flexCelPrintDocument1.DefaultPageSettings.PrinterSettings.PrintRange = PrintRange.AllPages;
                    maxPage = -1;
                }
                this.BringToFront();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return true;
        }

        private bool LoadPreferencesPdf()
        {
            //NOTE: THERE SHOULD BE *A LOT* MORE VALIDATION OF VALUES ON THIS METHOD. (For example, validate that margins are between bounds)
            // As this is a simple demo, they are not included. 
            try
            {
                ExcelFile Xls = flexCelPdfExport1.Workbook;
                Xls.PrintGridLines = chGridLines;
                Xls.PageHeader = edHeader;
                Xls.PageFooter = edFooter;
                Xls.ShowFormulaText = chFormulaText;

                if (chFitIn)
                {
                    Xls.PrintToFit = true;
                    Xls.PrintNumberOfHorizontalPages = Convert.ToInt32(edHPages);
                    Xls.PrintNumberOfVerticalPages = Convert.ToInt32(edVPages);
                }
                else
                    Xls.PrintToFit = false;

                //// hiển thị gọn trong 1 trang
                //Xls.PrintToFit = true;
                //Xls.PrintNumberOfHorizontalPages = 0;
                //Xls.PrintNumberOfVerticalPages = 0;

                if (chPrintLeft) Xls.PrintOptions |= TPrintOptions.LeftToRight;
                else Xls.PrintOptions &= ~TPrintOptions.LeftToRight;

                try
                {
                    Xls.PrintScale = (int)spinZoom.Value - 5;//edZoom - 5;// trackBarZoomPage.Value;
                }
                catch
                {
                    MessageBox.Show("Invalid Zoom");
                    return false;
                }

                //TXlsMargins m = new TXlsMargins();
                //if (leftMarginBox.EditValue != null
                //      && rightMarginBox.EditValue != null
                //      && topMarginBox.EditValue != null
                //      && bottomMarginBox.EditValue != null
                //      && headerMarginBox.EditValue != null
                //      && footerMarginBox.EditValue != null
                //      )
                //{
                //    m.Left = System.Convert.ToDouble(leftMarginBox.Value);
                //    m.Top = System.Convert.ToDouble(topMarginBox.Value);
                //    m.Right = System.Convert.ToDouble(rightMarginBox.Value);
                //    m.Bottom = System.Convert.ToDouble(bottomMarginBox.Value);
                //    m.Header = System.Convert.ToDouble(headerMarginBox.Value);
                //    m.Footer = System.Convert.ToDouble(footerMarginBox.Value);
                //    Xls.SetPrintMargins(m);
                //}

                TXlsMargins m = new TXlsMargins();
                m.Left = edl;
                m.Top = edt;
                m.Right = edr;
                m.Bottom = edb;
                m.Header = edh;
                m.Footer = edf;
                Xls.SetPrintMargins(m);

                Xls.PrintCopies = (int)numericUpDownCopies.Value;

                //flexCelPdfExport1.PrintRangeLeft = Convert.ToInt32(edLeft);
                //flexCelPdfExport1.PrintRangeTop = Convert.ToInt32(edTop);
                //flexCelPdfExport1.PrintRangeRight = Convert.ToInt32(edRight);
                //flexCelPdfExport1.PrintRangeBottom = Convert.ToInt32(edBottom);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return true;
        }

        public void FocusOnLoad()
        {
            try
            {
                numericUpDownCopies.Focus();
                numericUpDownCopies.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
