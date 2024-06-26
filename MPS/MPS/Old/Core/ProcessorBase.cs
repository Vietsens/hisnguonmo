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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace MPS.Core
{
    class ProcessorBase
    {
        RDOBase rdoBase;
        protected SAR_PRINT_TYPE config;
        protected SAR_PRINT print;
        protected Dictionary<string, object> singleValueDictionary = new Dictionary<string, object>();
        MPS.Printer.PreviewType previewType;
        string printerName;

        internal ProcessorBase(SAR_PRINT_TYPE configData, RDOBase rdoData, MPS.Printer.PreviewType previewType, string printerName)
        {
            this.config = configData;
            this.rdoBase = rdoData;
            this.previewType = previewType;
            this.printerName = printerName;
        }

        protected bool PrintPreview(MemoryStream streamResult, string fileTemplate)
        {
            try
            {
                if (streamResult != null && streamResult.Length > 0)
                {
                    streamResult.Position = 0;
                    Inventec.Common.Print.FlexCelPrintProcessor printProcess = new Inventec.Common.Print.FlexCelPrintProcessor(streamResult, this.printerName, fileTemplate, 1);
                    switch (previewType)
                    {
                        case Printer.PreviewType.Show:
                            printProcess.PrintPreviewShow();
                            break;
                        case Printer.PreviewType.ShowDialog:
                            printProcess.PrintPreview();
                            break;
                        case Printer.PreviewType.PrintNow:
                            printProcess.Print();
                            break;
                    }
                }
                else
                {
                    throw new ArgumentNullException("streamResult is null");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        protected bool PrintPreview(MemoryStream streamResult, string fileTemplate, int numCopy)
        {
            try
            {
                if (streamResult != null && streamResult.Length > 0)
                {
                    streamResult.Position = 0;
                    Inventec.Common.Print.FlexCelPrintProcessor printProcess = new Inventec.Common.Print.FlexCelPrintProcessor(streamResult, this.printerName, fileTemplate, numCopy);
                    switch (previewType)
                    {
                        case Printer.PreviewType.Show:
                            printProcess.PrintPreviewShow();
                            break;
                        case Printer.PreviewType.ShowDialog:
                            printProcess.PrintPreview();
                            break;
                        case Printer.PreviewType.PrintNow:
                            printProcess.Print();
                            break;
                    }
                }
                else
                {
                    throw new ArgumentNullException("streamResult is null");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        protected bool PrintPreview(MemoryStream streamResult, string fileTemplate, int numCopy, bool isPreview)
        {
            try
            {
                if (streamResult != null && streamResult.Length > 0)
                {
                    streamResult.Position = 0;
                    Inventec.Common.Print.FlexCelPrintProcessor printProcess = new Inventec.Common.Print.FlexCelPrintProcessor(streamResult, this.printerName, fileTemplate, numCopy, isPreview);
                    switch (previewType)
                    {
                        case Printer.PreviewType.Show:
                            printProcess.PrintPreviewShow();
                            break;
                        case Printer.PreviewType.ShowDialog:
                            printProcess.PrintPreview();
                            break;
                        case Printer.PreviewType.PrintNow:
                            printProcess.Print();
                            break;
                    }
                }
                else
                {
                    throw new ArgumentNullException("streamResult is null");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        protected void SetCommonSingleKey()
        {
            try
            {
                singleValueDictionary.Add(CommonKey._PARENT_ORGANIZATION_NAME, PrintValue.ParentOrganizationName);
                singleValueDictionary.Add(CommonKey._ORGANIZATION_NAME, PrintValue.OrganizationName);
                System.DateTime now = System.DateTime.Now;
                singleValueDictionary.Add(CommonKey._CURRENT_TIME_STR, now.ToString("dd/MM/yyyy HH:mm:ss"));
                singleValueDictionary.Add(CommonKey._CURRENT_DATE_STR, now.ToString("dd/MM/yyyy"));
                singleValueDictionary.Add(CommonKey._CURRENT_MONTH_STR, now.ToString("MM/yyyy"));
                singleValueDictionary.Add(CommonKey._CURRENT_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.SystemDateTimeToDateSeparateString(now));
                singleValueDictionary.Add(CommonKey._CURRENT_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.SystemDateTimeToTimeSeparateString(now));
                singleValueDictionary.Add(CommonKey._CURRENT_TIME_SEPARATE_BEGIN_TIME_STR, GlobalQuery.GetCurrentTimeSeparateBeginTime(now));
                singleValueDictionary.Add(CommonKey._CURRENT_TIME_SEPARATE_WITHOUT_SECOND_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Inventec.Common.DateTime.Get.Now() ?? 0));
                singleValueDictionary.Add(CommonKey._CURRENT_MONTH_SEPARATE_STR, Inventec.Common.DateTime.Convert.SystemDateTimeToMonthSeparateString(now));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void SetSingleKey()
        {
            try
            {
                if (rdoBase.keyValues != null && rdoBase.keyValues.Count > 0)
                {
                    foreach (var item in rdoBase.keyValues)
                    {
                        singleValueDictionary.Add(item.KEY, item.VALUE);
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
