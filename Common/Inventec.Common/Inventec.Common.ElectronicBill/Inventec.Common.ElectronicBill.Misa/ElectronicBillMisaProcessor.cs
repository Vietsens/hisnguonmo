using Inventec.Common.ElectronicBill.Misa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Inventec.Common.ElectronicBill.Misa
{
    class ElectronicBillMisaProcessor
    {
        public static IRun GetProcessor(Type type)
        {
            IRun result = null;
            switch (type)
            {
                case Type.CreateInvoice:
                    result = new Processor.CreateInvoice();
                    break;
                case Type.PreviewInvoice:
                    result = new Processor.PreviewInvoice();
                    break;
                case Type.SignInvoice:
                    result = new Processor.SignInvoice();
                    break;
                case Type.ReleaseInvoice:
                    result = new Processor.ReleaseInvoice();
                    break;
                case Type.ViewInvoice:
                    result = new Processor.ViewInvoice();
                    break;
                case Type.DeleteInvoice:
                    result = new Processor.DeleteInvoice();
                    break;
                case Type.ConvertInvoice:
                    result = new Processor.ConvertInvoice();
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
