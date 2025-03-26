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
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FlexCel.XlsAdapter;
using Inventec.Common.Logging;
using System.Drawing.Printing;
using Inventec.Common.FlexCelPrint;
using System.Windows.Forms;
using Inventec.Common.Print.Ado;

namespace Inventec.Common.Print
{
    public class FlexCelPrintProcessor
    {
        PrintNowProcess printNowProcess;
        frmSetupPrintPreview frmPrintPreview;
        Action showTutorialModule;
        List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> printMergeAdos;

        public FlexCelPrintProcessor()
        {

        }

        public FlexCelPrintProcessor(MemoryStream data, Inventec.Common.FlexCelPrint.DelegateEventLog eventLog, string defaultPrintName, string pathFileTemplate)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.1");
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("MemoryStream data print is null");

                data.Position = 0;
                frmPrintPreview = new frmSetupPrintPreview(data, eventLog, defaultPrintName, pathFileTemplate);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(string templatePath, Inventec.Common.FlexCelPrint.DelegateEventLog eventLog, string defaultPrintName, string pathFileTemplate)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.2");
                if (String.IsNullOrEmpty(templatePath))
                    throw new ArgumentNullException("templatePath is null");

                frmPrintPreview = new frmSetupPrintPreview(templatePath, eventLog, defaultPrintName, pathFileTemplate);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(MemoryStream data, Inventec.Common.FlexCelPrint.DelegateEventLog eventLog, string pathFileTemplate)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.3");
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("MemoryStream data print is null");

                data.Position = 0;
                frmPrintPreview = new frmSetupPrintPreview(data, eventLog, pathFileTemplate);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(string templatePath, Inventec.Common.FlexCelPrint.DelegateEventLog eventLog, string pathFileTemplate)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.4");
                if (String.IsNullOrEmpty(templatePath))
                    throw new ArgumentNullException("templatePath is null");

                frmPrintPreview = new frmSetupPrintPreview(templatePath, eventLog, pathFileTemplate);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(MemoryStream data, string pathFileTemplate)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.5");
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("MemoryStream data print is null");

                data.Position = 0;
                frmPrintPreview = new frmSetupPrintPreview(data, pathFileTemplate);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(MemoryStream data, string defaultPrintName, string pathFileTemplate)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.6");
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("MemoryStream data print is null");

                data.Position = 0;
                frmPrintPreview = new frmSetupPrintPreview(data, defaultPrintName, pathFileTemplate);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.7");
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("MemoryStream data print is null");

                data.Position = 0;
                frmPrintPreview = new frmSetupPrintPreview(data, defaultPrintName, pathFileTemplate, numCopy);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview)
            : this(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, true)
        {
            try
            {
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("MemoryStream data print is null");

                data.Position = 0;
                frmPrintPreview = new frmSetupPrintPreview(data, defaultPrintName, pathFileTemplate, numCopy, isPreview);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isAllowExport)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.8");
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("MemoryStream data print is null");

                data.Position = 0;
                frmPrintPreview = new frmSetupPrintPreview(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, isAllowExport);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isAllowExport, Dictionary<string, object> templateKey)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.9");
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("MemoryStream data print is null");

                data.Position = 0;
                frmPrintPreview = new frmSetupPrintPreview(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, isAllowExport, templateKey);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isAllowExport, Dictionary<string, object> templateKey, Inventec.Common.FlexCelPrint.DelegateEventLog eventLog)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.10");
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("MemoryStream data print is null");

                data.Position = 0;
                frmPrintPreview = new frmSetupPrintPreview(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, isAllowExport, templateKey, eventLog);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isAllowExport, Dictionary<string, object> templateKey, Inventec.Common.FlexCelPrint.DelegateEventLog eventLog, object emrInputADO)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.11");
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("MemoryStream data print is null");

                data.Position = 0;
                frmPrintPreview = new frmSetupPrintPreview(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, isAllowExport, templateKey, eventLog, emrInputADO);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isAllowExport, Dictionary<string, object> templateKey, Inventec.Common.FlexCelPrint.DelegateEventLog eventLog, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint eventPrint, object emrInputADO)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.12");
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("MemoryStream data print is null");

                data.Position = 0;
                frmPrintPreview = new frmSetupPrintPreview(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, isAllowExport, templateKey, eventLog, eventPrint, emrInputADO);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isAllowExport, Dictionary<string, object> templateKey, DelegateEventLog eventLog, DelegateReturnEventPrint eventPrint, object emrInputADO, DelegatePrintLog printLog, DelegateShowPrintLog showLog)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.13");
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("MemoryStream data print is null");

                data.Position = 0;
                frmPrintPreview = new frmSetupPrintPreview(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, isAllowExport, templateKey, eventLog, eventPrint, emrInputADO, printLog, showLog);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isAllowExport, Dictionary<string, object> templateKey, DelegateEventLog eventLog, DelegateReturnEventPrint eventPrint, object emrInputADO, DelegatePrintLog printLog, DelegateShowPrintLog showLog, bool isAllowEditTemplateFile)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.14");
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("MemoryStream data print is null");

                data.Position = 0;
                frmPrintPreview = new frmSetupPrintPreview(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, isAllowExport, templateKey, eventLog, eventPrint, emrInputADO, printLog, showLog, isAllowEditTemplateFile);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isAllowExport, Dictionary<string, object> templateKey, DelegateEventLog eventLog, DelegateReturnEventPrint eventPrint, object emrInputADO, DelegatePrintLog printLog, DelegateShowPrintLog showLog, bool isAllowEditTemplateFile, bool isSingleCopy, bool isPrintNow = false)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.15");
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("MemoryStream data print is null");
                data.Position = 0;

                if (isPrintNow && PrintCFG.IDEPrintType == PrintCFG.IDEPrintType__Apose)
                    printNowProcess = new PrintNowProcess(data, defaultPrintName, numCopy, eventLog, eventPrint, printLog);
                else
                    frmPrintPreview = new frmSetupPrintPreview(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, isAllowExport, templateKey, eventLog, eventPrint, emrInputADO, printLog, showLog, isAllowEditTemplateFile, isSingleCopy);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isAllowExport, Dictionary<string, object> templateKey, DelegateEventLog eventLog, DelegateReturnEventPrint eventPrint, object emrInputADO, DelegatePrintLog printLog, DelegateShowPrintLog showLog, DelegateGetNumOrderPrint getNumOrderPrint, bool isAllowEditTemplateFile, bool isSingleCopy, bool isPrintNow = false, bool isPrintExceptionReason = false)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.15");
                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("MemoryStream data print is null");
                data.Position = 0;

                if (isPrintNow && PrintCFG.IDEPrintType == PrintCFG.IDEPrintType__Apose)
                    printNowProcess = new PrintNowProcess(data, defaultPrintName, numCopy, eventLog, eventPrint, printLog);
                else
                    frmPrintPreview = new frmSetupPrintPreview(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, isAllowExport, templateKey, eventLog, eventPrint, emrInputADO, printLog, showLog, isAllowEditTemplateFile, isSingleCopy, getNumOrderPrint, isPrintExceptionReason);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(string templatePath, string defaultPrintName, string pathFileTemplate)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.16");
                if (String.IsNullOrEmpty(templatePath))
                    throw new ArgumentNullException("templatePath is null");

                frmPrintPreview = new frmSetupPrintPreview(templatePath, defaultPrintName, pathFileTemplate);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(string templatePath, string pathFileTemplate)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.17");
                if (String.IsNullOrEmpty(templatePath))
                    throw new ArgumentNullException("templatePath is null");

                frmPrintPreview = new frmSetupPrintPreview(templatePath, pathFileTemplate);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(string templatePath, string defaultPrintName, string pathFileTemplate, int numCopy)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.18");
                if (String.IsNullOrEmpty(templatePath))
                    throw new ArgumentNullException("templatePath is null");

                frmPrintPreview = new frmSetupPrintPreview(templatePath, defaultPrintName, pathFileTemplate, numCopy);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(string templatePath, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview)
            : this(templatePath, defaultPrintName, pathFileTemplate, numCopy, isPreview, true)
        {
        }

        public FlexCelPrintProcessor(string templatePath, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isAllowExport)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.19");
                if (String.IsNullOrEmpty(templatePath))
                    throw new ArgumentNullException("templatePath is null");

                frmPrintPreview = new frmSetupPrintPreview(templatePath, defaultPrintName, pathFileTemplate, numCopy, isPreview, isAllowExport);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(string templatePath, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isAllowExport, Dictionary<string, object> templateKey)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.20");
                if (String.IsNullOrEmpty(templatePath))
                    throw new ArgumentNullException("templatePath is null");

                frmPrintPreview = new frmSetupPrintPreview(templatePath, defaultPrintName, pathFileTemplate, numCopy, isPreview, isAllowExport, templateKey);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(string templatePath, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isAllowExport, Dictionary<string, object> templateKey, DelegatePrintLog printLog, DelegateShowPrintLog showLog)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.21");
                if (String.IsNullOrEmpty(templatePath))
                    throw new ArgumentNullException("templatePath is null");

                frmPrintPreview = new frmSetupPrintPreview(templatePath, defaultPrintName, pathFileTemplate, numCopy, isPreview, isAllowExport, templateKey, null, null, null, printLog, showLog);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public FlexCelPrintProcessor(string templatePath, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isAllowExport, Dictionary<string, object> templateKey, DelegatePrintLog printLog, DelegateShowPrintLog showLog, bool isSingleCopy)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FlexCelPrintProcessor.22");
                if (String.IsNullOrEmpty(templatePath))
                    throw new ArgumentNullException("templatePath is null");

                frmPrintPreview = new frmSetupPrintPreview(templatePath, defaultPrintName, pathFileTemplate, numCopy, isPreview, isAllowExport, templateKey, null, null, null, printLog, showLog, isSingleCopy);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public void SetPartialFile(List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> printMerges)
        {
            this.printMergeAdos = printMerges;
        }

        public void SetDelegateOpenTutorial(Action _showTutorialModule)
        {
            try
            {
                if (_showTutorialModule != null)
                {
                    this.showTutorialModule = _showTutorialModule;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public void Print()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("Print now begin");
                if (this.printMergeAdos != null && this.printMergeAdos.Count > 0)
                {
                    if (this.printMergeAdos.Count > 1)
                    {
                        frmSetupPrintPreviewMerge frmPrintPreview = new frmSetupPrintPreviewMerge("", this.printMergeAdos[0].printerName, this.printMergeAdos[0].fileName, this.printMergeAdos[0].numCopy, false, this.printMergeAdos[0].isAllowExport, this.printMergeAdos[0].TemplateKey, this.printMergeAdos[0].eventLog, this.printMergeAdos[0].eventPrint, this.printMergeAdos[0].EmrInputADO, this.printMergeAdos[0].PrintLog, this.printMergeAdos[0].ShowPrintLog, this.printMergeAdos[0].IsSingleCopy);
                        if (this.printMergeAdos[0].ShowTutorialModule != null)
                            frmPrintPreview.SetTutorialModule(this.printMergeAdos[0].ShowTutorialModule);
                        frmPrintPreview.SetPartialFileList(this.printMergeAdos);
                        frmPrintPreview.Print();
                        return;
                    }
                    else
                    {
                        if (this.printMergeAdos[0].saveMemoryStream != null && this.printMergeAdos[0].saveMemoryStream.Length > 0)
                        {
                            this.printMergeAdos[0].saveMemoryStream.Position = 0;                            
                            frmPrintPreview = new frmSetupPrintPreview(this.printMergeAdos[0].saveMemoryStream, this.printMergeAdos[0].printerName, this.printMergeAdos[0].fileName, this.printMergeAdos[0].numCopy, false, this.printMergeAdos[0].isAllowExport, this.printMergeAdos[0].TemplateKey, this.printMergeAdos[0].eventLog, this.printMergeAdos[0].eventPrint, this.printMergeAdos[0].EmrInputADO, this.printMergeAdos[0].PrintLog, this.printMergeAdos[0].ShowPrintLog, this.printMergeAdos[0].IsSingleCopy);
                        }
                        else if (!String.IsNullOrEmpty(this.printMergeAdos[0].saveFilePath) && File.Exists(this.printMergeAdos[0].saveFilePath))
                        {
                            frmPrintPreview = new frmSetupPrintPreview(this.printMergeAdos[0].saveFilePath, this.printMergeAdos[0].printerName, this.printMergeAdos[0].fileName, this.printMergeAdos[0].numCopy, false, this.printMergeAdos[0].isAllowExport, this.printMergeAdos[0].TemplateKey, this.printMergeAdos[0].eventLog, this.printMergeAdos[0].eventPrint, this.printMergeAdos[0].EmrInputADO, this.printMergeAdos[0].PrintLog, this.printMergeAdos[0].ShowPrintLog, this.printMergeAdos[0].IsSingleCopy);
                        }
                    }
                }

                if (frmPrintPreview == null)
                    throw new ArgumentNullException("frmPrintPreview is null");

                if (this.showTutorialModule != null)
                    frmPrintPreview.SetTutorialModule(this.showTutorialModule);

                if (PrintCFG.IDEPrintType == PrintCFG.IDEPrintType__Flexcel)
                {
                    frmPrintPreview.Print();
                }
                else
                {
                    printNowProcess.PrintNow();
                }

                Inventec.Common.Logging.LogSystem.Debug("Print now end");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public void PrintPreview()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("PrintPreview begin");
                if (this.printMergeAdos != null && this.printMergeAdos.Count > 0)
                {
                    if (this.printMergeAdos.Count > 1)
                    {
                        frmSetupPrintPreviewMerge frmPrintPreview = new frmSetupPrintPreviewMerge("", this.printMergeAdos[0].printerName, this.printMergeAdos[0].fileName, this.printMergeAdos[0].numCopy, false, this.printMergeAdos[0].isAllowExport, this.printMergeAdos[0].TemplateKey, this.printMergeAdos[0].eventLog, this.printMergeAdos[0].eventPrint, this.printMergeAdos[0].EmrInputADO, this.printMergeAdos[0].PrintLog, this.printMergeAdos[0].ShowPrintLog, this.printMergeAdos[0].IsSingleCopy);
                        if (this.printMergeAdos[0].ShowTutorialModule != null)
                            frmPrintPreview.SetTutorialModule(this.printMergeAdos[0].ShowTutorialModule);
                        frmPrintPreview.SetPartialFileList(this.printMergeAdos);
                        frmPrintPreview.ShowDialog(frmPrintPreview.ParentForm);
                        frmPrintPreview.FocusOnLoad();
                        frmPrintPreview.Activate();
                        frmPrintPreview.FormClosed += FormClosed;
                        return;
                    }
                    else
                    {
                        if (this.printMergeAdos[0].saveMemoryStream != null && this.printMergeAdos[0].saveMemoryStream.Length > 0)
                        {
                            this.printMergeAdos[0].saveMemoryStream.Position = 0;
                            frmPrintPreview = new frmSetupPrintPreview(this.printMergeAdos[0].saveMemoryStream, this.printMergeAdos[0].printerName, this.printMergeAdos[0].fileName, this.printMergeAdos[0].numCopy, false, this.printMergeAdos[0].isAllowExport, this.printMergeAdos[0].TemplateKey, this.printMergeAdos[0].eventLog, this.printMergeAdos[0].eventPrint, this.printMergeAdos[0].EmrInputADO, this.printMergeAdos[0].PrintLog, this.printMergeAdos[0].ShowPrintLog, this.printMergeAdos[0].IsSingleCopy);
                        }
                        else if (!String.IsNullOrEmpty(this.printMergeAdos[0].saveFilePath) && File.Exists(this.printMergeAdos[0].saveFilePath))
                        {
                            frmPrintPreview = new frmSetupPrintPreview(this.printMergeAdos[0].saveFilePath, this.printMergeAdos[0].printerName, this.printMergeAdos[0].fileName, this.printMergeAdos[0].numCopy, false, this.printMergeAdos[0].isAllowExport, this.printMergeAdos[0].TemplateKey, this.printMergeAdos[0].eventLog, this.printMergeAdos[0].eventPrint, this.printMergeAdos[0].EmrInputADO, this.printMergeAdos[0].PrintLog, this.printMergeAdos[0].ShowPrintLog, this.printMergeAdos[0].IsSingleCopy);
                        }
                    }
                }

                if (frmPrintPreview == null)
                    throw new ArgumentNullException("frmPrintPreview is null");

                if (this.showTutorialModule != null)
                    frmPrintPreview.SetTutorialModule(this.showTutorialModule);

                frmPrintPreview.ShowDialog(frmPrintPreview.Owner);
                frmPrintPreview.FocusOnLoad();
                frmPrintPreview.Activate();
                frmPrintPreview.FormClosed += FormClosed;

                Inventec.Common.Logging.LogSystem.Debug("PrintPreview end");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public void PrintPreviewShow()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("PrintPreviewShow begin");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("printMergeAdos.Count", printMergeAdos != null ? printMergeAdos.Count : 0));
                if (this.printMergeAdos != null && this.printMergeAdos.Count > 0)
                {
                    if (this.printMergeAdos.Count > 1)
                    {
                        frmSetupPrintPreviewMerge frmPrintPreview = new frmSetupPrintPreviewMerge("", this.printMergeAdos[0].printerName, this.printMergeAdos[0].fileName, this.printMergeAdos[0].numCopy, false, this.printMergeAdos[0].isAllowExport, this.printMergeAdos[0].TemplateKey, this.printMergeAdos[0].eventLog, this.printMergeAdos[0].eventPrint, this.printMergeAdos[0].EmrInputADO, this.printMergeAdos[0].PrintLog, this.printMergeAdos[0].ShowPrintLog, this.printMergeAdos[0].IsSingleCopy);

                        if (this.printMergeAdos[0].ShowTutorialModule != null)
                            frmPrintPreview.SetTutorialModule(this.printMergeAdos[0].ShowTutorialModule);
                        frmPrintPreview.SetPartialFileList(this.printMergeAdos);
                        frmPrintPreview.Show(frmPrintPreview.ParentForm);
                        frmPrintPreview.FocusOnLoad();
                        frmPrintPreview.Activate();
                        frmPrintPreview.FormClosed += FormClosed;
                        return;
                    }
                    else
                    {
                        if (this.printMergeAdos[0].saveMemoryStream != null && this.printMergeAdos[0].saveMemoryStream.Length > 0)
                        {
                            this.printMergeAdos[0].saveMemoryStream.Position = 0;
                            frmPrintPreview = new frmSetupPrintPreview(this.printMergeAdos[0].saveMemoryStream, this.printMergeAdos[0].printerName, this.printMergeAdos[0].fileName, this.printMergeAdos[0].numCopy, false, this.printMergeAdos[0].isAllowExport, this.printMergeAdos[0].TemplateKey, this.printMergeAdos[0].eventLog, this.printMergeAdos[0].eventPrint, this.printMergeAdos[0].EmrInputADO, this.printMergeAdos[0].PrintLog, this.printMergeAdos[0].ShowPrintLog, this.printMergeAdos[0].IsSingleCopy);
                        }
                        else if (!String.IsNullOrEmpty(this.printMergeAdos[0].saveFilePath) && File.Exists(this.printMergeAdos[0].saveFilePath))
                        {
                            frmPrintPreview = new frmSetupPrintPreview(this.printMergeAdos[0].saveFilePath, this.printMergeAdos[0].printerName, this.printMergeAdos[0].fileName, this.printMergeAdos[0].numCopy, false, this.printMergeAdos[0].isAllowExport, this.printMergeAdos[0].TemplateKey, this.printMergeAdos[0].eventLog, this.printMergeAdos[0].eventPrint, this.printMergeAdos[0].EmrInputADO, this.printMergeAdos[0].PrintLog, this.printMergeAdos[0].ShowPrintLog, this.printMergeAdos[0].IsSingleCopy);
                        }
                    }
                }

                if (frmPrintPreview == null)
                    throw new ArgumentNullException("frmPrintPreview is null");

                if (this.showTutorialModule != null)
                    frmPrintPreview.SetTutorialModule(this.showTutorialModule);

                frmPrintPreview.Show(frmPrintPreview.ParentForm);
                frmPrintPreview.FocusOnLoad();
                frmPrintPreview.Activate();
                frmPrintPreview.FormClosed += FormClosed;

                Inventec.Common.Logging.LogSystem.Debug("PrintPreviewShow end");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Form active = null;
                var a = Application.OpenForms.Cast<Form>().ToList();
                if (a != null && a.Count > 0)
                {
                    foreach (var item in a)
                    {
                        if (item.ActiveControl == null) continue;
                        if (item.ContainsFocus)
                        {
                            active = item;
                            break;
                        }
                    }
                    if (active == null)
                    {
                        for (int i = (Application.OpenForms.Count - 1); i >= 0; i--)
                        {
                            if (Application.OpenForms[i].Name == "frmWaitForm" || String.IsNullOrEmpty(Application.OpenForms[i].Name)) continue;
                            if (Application.OpenForms[i] == frmPrintPreview) continue;

                            active = Application.OpenForms[i];
                            break;
                        }
                    }
                }

                if (active != null)
                {
                    active.Activate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
