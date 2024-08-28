using Inventec.Common.Logging;
using Inventec.Common.QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Inventec.Common.FlexCellExport
{
    public class ProcessQRCodeTag
    {
        public bool ProcessData(Store store, Dictionary<string, BarcodeLib.Barcode> dicData)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.flexCel == null) throw new ArgumentNullException("store.flexCel");
                if (dicData == null || dicData.Count == 0) throw new ArgumentNullException("dicData");
                if (store.DictionaryTemplateKey == null)
                    store.DictionaryTemplateKey = new Dictionary<string, object>();

                foreach (KeyValuePair<string, BarcodeLib.Barcode> pair in dicData)
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(pair.Value.RawData, QRCodeGenerator.ECCLevel.Q);
                    BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                    byte[] qrCodeImage = qrCode.GetGraphic(20);

                    store.flexCel.SetValue(pair.Key, qrCodeImage);

                    store.DictionaryTemplateKey[pair.Key] = "QR CODE " + pair.Key;

                    qrCode.Dispose();
                    qrCodeData.Dispose();
                    qrGenerator.Dispose();
                }

                result = true;
            }
            catch (ArgumentNullException ex)
            {
                LogSystem.Warn(ex);
                result = false;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
