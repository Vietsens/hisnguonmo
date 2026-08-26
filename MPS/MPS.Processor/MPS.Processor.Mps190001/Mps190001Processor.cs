using FlexCel.Report;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps190001.PDO;
using Inventec.Core;
using MPS.ProcessorBase;
using MOS.EFMODEL.DataModels;
using System.Text;
using System.Linq;
using Inventec.Common.QRCoder;

namespace MPS.Processor.Mps190001
{
    class Mps190001Processor : AbstractProcessor
    {
        Mps190001PDO rdo;
        List<ServiceReqADO> ListAdo = new List<ServiceReqADO>();

        public Mps190001Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps190001PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (rdo != null)
                {
                    if (rdo.lstServiceReq != null)
                    {
                        //if (!String.IsNullOrWhiteSpace(rdo.lstServiceReq.TDL_PATIENT_CODE))
                        //{
                        //    Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.lstServiceReq.TDL_PATIENT_CODE);
                        //    barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        //    barcodePatientCode.IncludeLabel = false;
                        //    barcodePatientCode.Width = 120;
                        //    barcodePatientCode.Height = 40;
                        //    barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        //    barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        //    barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        //    barcodePatientCode.IncludeLabel = true;

                        //    dicImage.Add(Mps190001ExtendSingleKey.PATIENT_CODE_BAR, barcodePatientCode);
                        //}

                        //if (!String.IsNullOrWhiteSpace(rdo.lstServiceReq.SERVICE_REQ_CODE))
                        //{
                        //    Inventec.Common.BarcodeLib.Barcode barcodeServiceReq = new Inventec.Common.BarcodeLib.Barcode(rdo.lstServiceReq.SERVICE_REQ_CODE);
                        //    barcodeServiceReq.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        //    barcodeServiceReq.IncludeLabel = false;
                        //    barcodeServiceReq.Width = 120;
                        //    barcodeServiceReq.Height = 40;
                        //    barcodeServiceReq.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        //    barcodeServiceReq.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        //    barcodeServiceReq.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        //    barcodeServiceReq.IncludeLabel = true;

                        //    dicImage.Add(Mps190001ExtendSingleKey.BARCODE_SERVICE_REQ_CODE, barcodeServiceReq);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                SetBarcodeKey();
                SetSingleKey();
                ProcessListData();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                rdo.lstServiceReq = rdo.lstServiceReq.OrderBy(o => o.EXECUTE_ROOM_ID).ThenBy(p => p.TDL_PATIENT_NAME).ToList();
                objectTag.AddObjectData(store, "lstServiceReq", rdo.lstServiceReq);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetSingleKey()
        {
            try
            {

                if (rdo.lstServiceReq != null)
                {
                    //QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    //QRCodeData qrCodeData = qrGenerator.CreateQrCode(totalInfo, QRCodeGenerator.ECCLevel.Q);
                    //BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                    //byte[] qrCodeImage = qrCode.GetGraphic(20);
                    //SetSingleKey(Mps190001ExtendSingleKey.QRCODE_PATIENT, qrCodeImage);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessListData()
        {
            try
            {
                if (rdo.lstServiceReq != null && rdo.lstServiceReq.Count() > 0)
                {
                    foreach (var item in rdo.lstServiceReq)
                    {
                        if (!String.IsNullOrWhiteSpace(item.TDL_TREATMENT_CODE))
                        {
                            Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(item.TDL_TREATMENT_CODE);
                            barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                            barcodePatientCode.IncludeLabel = false;
                            barcodePatientCode.Width = 60;
                            barcodePatientCode.Height = 20;
                            barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                            barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                            barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                            barcodePatientCode.IncludeLabel = true;

                            dicImage.Add(item.SERVICE_REQ_CODE+ "_BAR", barcodePatientCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        class CustomerFuncRownumberData : TFlexCelUserFunction
        {
            public CustomerFuncRownumberData()
            {
            }
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                long result = 0;
                try
                {
                    long rownumber = Convert.ToInt64(parameters[0]);
                    result = (rownumber + 1);
                }
                catch (Exception ex)
                {
                    LogSystem.Debug(ex);
                }

                return result;
            }
        }
    }
}
