using FlexCel.Report;
using Inventec.Common.QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCellExport
{
    class TFlexCelUFCreateQR : TFlexCelUserFunction
    {
        public override object Evaluate(object[] parameters)
        {
            object result = null;
            if (parameters == null || parameters.Length <= 0)
                return result;

            try
            {
                string data = parameters[0].ToString();
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);

                result = qrCode.GetGraphic(20);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
