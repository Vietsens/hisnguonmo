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
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCelPrint.Ado
{
    public class PrintMergeAdo
    {
        public PrintMergeAdo() { }

        public string printTypeCode { get; set; }
        public string fileName { get; set; }
        public object data { get; set; }
        public string printerName { get; set; }
        public int numCopy { get; set; }
        public string saveFilePath { get; set; }
        public MemoryStream saveMemoryStream { get; set; }
        public bool isAllowExport { get; set; }
        public Inventec.Common.FlexCelPrint.DelegateEventLog eventLog { get; set; }
        public Inventec.Common.FlexCelPrint.DelegatePrintLog PrintLog { get; set; }
        public Inventec.Common.FlexCelPrint.DelegateReturnEventPrint eventPrint { get; set; }
        public Inventec.Common.FlexCelPrint.DelegateShowPrintLog ShowPrintLog { get; set; }
        public Action<string, string> ActShowPrintLog { get; set; }
        public Action ShowTutorialModule { get; set; }
        public Dictionary<string, object> TemplateKey { get; set; }
        public bool IsSingleCopy { get; set; }
        public object EmrInputADO { get; set; }
        public bool IsAllowEditTemplateFile { get; set; }
        public bool IsPdfFile { get; set; }
    }
}
