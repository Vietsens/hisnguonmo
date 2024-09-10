using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.TemplaterExport
{
    public class ProcessBarCodeTag
    {
        public bool ProcessData(Store store, Dictionary<string, BarcodeLib.Barcode> dicData)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.templateDoc == null) throw new ArgumentNullException("store.templateDoc");
                if (dicData == null || dicData.Count == 0) throw new ArgumentNullException("dicData");
                if (store.DictionaryTemplateKey == null)
                    store.DictionaryTemplateKey = new Dictionary<string, object>();

                foreach (KeyValuePair<string, BarcodeLib.Barcode> pair in dicData)
                {
                    try
                    {
                        if (!System.String.IsNullOrEmpty(pair.Value.RawData))
                        {
                            Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode();
                            barcode.Alignment = pair.Value.Alignment;
                            barcode.IncludeLabel = pair.Value.IncludeLabel;
                            barcode.RotateFlipType = pair.Value.RotateFlipType;
                            barcode.LabelPosition = pair.Value.LabelPosition;
                            barcode.EncodedType = pair.Value.EncodedType;
                            barcode.LabelFont = new Font(barcode.LabelFont.FontFamily, 20);
                            var imgBarcode = barcode.Encode(barcode.EncodedType, pair.Value.RawData, pair.Value.Width, pair.Value.Height);
                            store.templateDoc.Templater.Replace(pair.Key, imgBarcode);
                            store.DictionaryTemplateKey[pair.Key] = "BAR CODE " + pair.Key;
                        }
                        else
                        {
                            LogSystem.Warn("Du lieu truyen vao khong hop le. RawData = " + pair.Value.RawData);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Error(ex);
                    }
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
