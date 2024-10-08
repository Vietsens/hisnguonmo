﻿using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Inventec.Common.FlexCellExport
{
    public class ProcessBarCodeTag
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
                    try
                    {
                        if (!System.String.IsNullOrEmpty(pair.Value.RawData))
                        {
                            Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode();
                            barcode.Alignment = (pair.Value.Alignment != null ? pair.Value.Alignment : Inventec.Common.BarcodeLib.AlignmentPositions.CENTER);
                            barcode.IncludeLabel = pair.Value.IncludeLabel;
                            barcode.RotateFlipType = (pair.Value.RotateFlipType != null ? pair.Value.RotateFlipType : System.Drawing.RotateFlipType.RotateNoneFlipNone);
                            barcode.LabelPosition = (pair.Value.LabelPosition != null ? pair.Value.LabelPosition : Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER);
                            barcode.EncodedType = (pair.Value.EncodedType != null ? pair.Value.EncodedType : BarcodeLib.TYPE.CODE128);
                            //barcode.LabelFont = new Font(barcode.LabelFont.FontFamily, 20);
                            pair.Value.Width = (pair.Value.Width > 0 ? pair.Value.Width : 100);
                            pair.Value.Height = (pair.Value.Height > 0 ? pair.Value.Height : 100);

                            store.flexCel.SetValue(pair.Key, Common.ConverterImageToArray(barcode.Encode(barcode.EncodedType, pair.Value.RawData, pair.Value.Width, pair.Value.Height)));
                            store.DictionaryTemplateKey[pair.Key] = "BAR CODE " + pair.Key;

                            barcode.Dispose();
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
