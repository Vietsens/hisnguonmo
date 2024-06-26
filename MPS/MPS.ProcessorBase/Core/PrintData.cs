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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.ProcessorBase.Core
{
    public class PrintData
    {
        public string printTypeCode { get; set; }
        public string fileName { get; set; }
        public object data { get; set; }
        public MPS.ProcessorBase.PrintConfig.PreviewType previewType { get; set; }
        public MPS.ProcessorBase.PrintConfig.CallPrintType callPrintType { get; set; }
        public string printerName { get; set; }
        public int numCopy { get; set; }
        public string saveFilePath { get; set; }
        public MemoryStream saveMemoryStream { get; set; }
        public bool isPreview { get; set; }
        public bool isAllowExport { get; set; }
        public Inventec.Common.FlexCelPrint.DelegateEventLog eventLog { get; set; }
        public Inventec.Common.FlexCelPrint.DelegateReturnEventPrint eventPrint { get; set; }
        public PrintConfig.DelegateShowPrintLog ShowPrintLog { get; set; }
        public Action ShowTutorialModule { get; set; }
        public object EmrInputADO { get; set; }
        public PrintStatus PrintMerge { get; set; }
        public bool isUserWordContent = true;

        public PrintData(string printTypeCode, string fileName, object data, MPS.ProcessorBase.PrintConfig.PreviewType previewType, string printerName)
            : this(printTypeCode, fileName, data, previewType, printerName, 1)
        {

        }

        public PrintData(string printTypeCode, string fileName, object data, MPS.ProcessorBase.PrintConfig.PreviewType previewType, string printerName, bool isUserWordContent)
            : this(printTypeCode, fileName, data, previewType, printerName, 1)
        {
            this.isUserWordContent = isUserWordContent;
        }

        public PrintData(string printTypeCode, string fileName, object data, MPS.ProcessorBase.PrintConfig.PreviewType previewType, string printerName, Inventec.Common.FlexCelPrint.DelegateEventLog eventLog)
            : this(printTypeCode, fileName, data, previewType, printerName, 1)
        {
            this.eventLog = eventLog;
        }

        public PrintData(string printTypeCode, string fileName, object data, MPS.ProcessorBase.PrintConfig.PreviewType previewType, string printerName, int numcopy)
            : this(printTypeCode, fileName, data, previewType, printerName, numcopy, null, null, false)
        {

        }

        public PrintData(string printTypeCode, string fileName, object data, MPS.ProcessorBase.PrintConfig.PreviewType previewType, string printerName, int numcopy, bool isPreview)
            : this(printTypeCode, fileName, data, previewType, printerName, numcopy, null, null, isPreview)
        {

        }

        public PrintData(string printTypeCode, string fileName, object data, MPS.ProcessorBase.PrintConfig.PreviewType previewType, string printerName, int numcopy, bool isPreview, bool isallowExport)
            : this(printTypeCode, fileName, data, previewType, MPS.ProcessorBase.PrintConfig.CallPrintType.Flexcel, printerName, numcopy, "", null, isPreview, isallowExport)
        {

        }

        public PrintData(string printTypeCode, string fileName, object data, MPS.ProcessorBase.PrintConfig.PreviewType previewType, string printerName, int numcopy, string savefilepath)
            : this(printTypeCode, fileName, data, previewType, printerName, numcopy, savefilepath, null, false)
        {

        }

        public PrintData(string printTypeCode, string fileName, object data, MPS.ProcessorBase.PrintConfig.PreviewType previewType, string printerName, int numcopy, MemoryStream savememorystream)
            : this(printTypeCode, fileName, data, previewType, printerName, numcopy, null, savememorystream, false)
        {

        }

        private PrintData(string printTypeCode, string fileName, object data, MPS.ProcessorBase.PrintConfig.PreviewType previewType, string printerName, int numcopy, string savefilepath, MemoryStream savememorystream, bool isPreview)
            : this(printTypeCode, fileName, data, previewType, MPS.ProcessorBase.PrintConfig.CallPrintType.Flexcel, printerName, numcopy, savefilepath, savememorystream, isPreview, false)
        {

        }

        private PrintData(string printTypeCode, string fileName, object data, MPS.ProcessorBase.PrintConfig.PreviewType previewType, MPS.ProcessorBase.PrintConfig.CallPrintType callPrintType, string printerName, int numcopy, string savefilepath, MemoryStream savememorystream, bool isPreview, bool isallowExport)
        {
            this.printTypeCode = printTypeCode;
            this.fileName = fileName;
            this.data = data;
            this.previewType = previewType;
            this.callPrintType = callPrintType;
            this.printerName = printerName;
            this.numCopy = numcopy;
            this.saveFilePath = savefilepath;
            this.saveMemoryStream = savememorystream;
            this.isPreview = isPreview;
            this.isAllowExport = isallowExport;
        }

        /// <summary>
        /// 
        /// </summary>
        public PrintData(string printTypeCode, string fileName, object data, MPS.ProcessorBase.PrintConfig.PreviewType previewType, string printerName, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint eventPrint)
            : this(printTypeCode, fileName, data, previewType, printerName, 1)
        {
            this.eventPrint = eventPrint;
        }

        public PrintData(string printTypeCode, string fileName, object data, MPS.ProcessorBase.PrintConfig.PreviewType previewType, string printerName, PrintConfig.DelegateShowPrintLog showPrintLog)
            : this(printTypeCode, fileName, data, previewType, printerName, 1)
        {
            this.ShowPrintLog = showPrintLog;
        }

    }
}
