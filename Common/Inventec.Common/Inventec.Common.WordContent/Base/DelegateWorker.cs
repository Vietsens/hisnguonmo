using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.WordContent.Base
{
    public delegate bool DelegateUpdateTableReference(SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated);
    public delegate List<long> DelegateGetListFromPrintReference();
    public delegate void DelegateShowPrintPreviewAfterSave(byte[] CONTENT_B);
    public delegate bool DelegateRunPrinter(string printCode, string fileName);
}
