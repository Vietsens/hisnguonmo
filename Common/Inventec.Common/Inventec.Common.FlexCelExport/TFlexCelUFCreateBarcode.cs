using FlexCel.Report;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCellExport
{
    class TFlexCelUFCreateBarcode : TFlexCelUserFunction
    {
        public override object Evaluate(object[] parameters)
        {
            object result = null;
            if (parameters == null || parameters.Length <= 0)
                return result;

            try
            {
                //Inventec.Common.Logging.LogSystem.Info("TFlexCelUFCreateBarcode");
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => parameters), parameters));

                string data = parameters[0].ToString();
                if (!string.IsNullOrWhiteSpace(data))
                {
                    Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode(data);
                    barcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    //barcode.IncludeLabel = true;
                    barcode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    int Width = 120;
                    int Height = 40;

                    if (parameters.Length > 2)
                    {
                        int.TryParse(parameters[1].ToString(), out Width);
                        int.TryParse(parameters[2].ToString(), out Height);
                    }

                    int includeLabel = 0;
                    if (parameters.Length > 3)
                    {
                        int.TryParse(parameters[3].ToString(), out includeLabel);
                    }

                    barcode.IncludeLabel = includeLabel == 0;

                    barcode.Width = Width > 0 ? Width : 120;
                    barcode.Height = Height > 0 ? Height : 40;

                    result = Common.ConverterImageToArray(barcode.Encode(barcode.EncodedType, data, barcode.Width, barcode.Height));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
