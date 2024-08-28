using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCelPrint.DocumentPrint
{
    class ThreadPrint
    {
        FlexCel.Render.FlexCelPrintDocument Document;

        public ThreadPrint(FlexCel.Render.FlexCelPrintDocument document)
        {
            this.Document = document;
        }

        public void Print()
        {
            Thread thread = new System.Threading.Thread(ProcessPrintNewThread);
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void ProcessPrintNewThread()
        {
            try
            {
                if (Document != null)
                {
                    using (Document)
                    {
                        if (FlexCelPrintUtil.GetConfigValue())
                        {
                            Document.PrintController = new StandardPrintController();
                        }
                        Document.Print();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
