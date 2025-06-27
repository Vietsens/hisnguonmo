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
using FlexCel.Report;
using Inventec.Common.Logging;
using Inventec.Common.QRCoder;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000502.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace MPS.Processor.Mps000502
{
    public partial class Mps000502Processor : AbstractProcessor
    {
        Mps000502PDO rdo;
        
        public Mps000502Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            if (rdoBase != null && rdoBase is Mps000502PDO)
            {
                rdo = (Mps000502PDO)rdoBase;
            }
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetBarcodeKey();
                SetSingleKey();

                this.SetSignatureKeyImageByCFG();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "SereNmse", rdo.lstSereNmse);
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo.Treatment != null && !string.IsNullOrEmpty(rdo.Treatment.TDL_PATIENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Treatment.TDL_PATIENT_CODE);
                    barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodePatientCode.IncludeLabel = false;
                    barcodePatientCode.Width = 120;
                    barcodePatientCode.Height = 40;
                    barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodePatientCode.IncludeLabel = true;
                    dicImage.Add(Mps000502ExtendSingleKey.PATIENT_CODE_BAR, barcodePatientCode);
                }

                if (rdo.ServiceReq != null && !string.IsNullOrEmpty(rdo.ServiceReq.SERVICE_REQ_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeServiceReqCode = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReq.SERVICE_REQ_CODE);
                    barcodeServiceReqCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeServiceReqCode.IncludeLabel = false;
                    barcodeServiceReqCode.Width = 120;
                    barcodeServiceReqCode.Height = 40;
                    barcodeServiceReqCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeServiceReqCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeServiceReqCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeServiceReqCode.IncludeLabel = true;
                    dicImage.Add(Mps000502ExtendSingleKey.SERVICE_REQ_CODE_BAR, barcodeServiceReqCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo.Treatment != null)
                {
                    
                    SetSingleKey(new KeyValue(Mps000502ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.AgeCaption(rdo.Treatment.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000502ExtendSingleKey.STR_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.Treatment.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000502ExtendSingleKey.STR_YEAR, rdo.Treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                }

                if (rdo.ServiceReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000502ExtendSingleKey.INSTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReq.INTRUCTION_TIME)));
               

                    SetSingleKey(new KeyValue(Mps000502ExtendSingleKey.FINISH_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        rdo.ServiceReq.FINISH_TIME ?? 0) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000502ExtendSingleKey.FINISH_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.ServiceReq.FINISH_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000502ExtendSingleKey.INTRUCTION_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        rdo.ServiceReq.INTRUCTION_TIME) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000502ExtendSingleKey.INTRUCTION_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        rdo.ServiceReq.INTRUCTION_TIME)));

                    SetSingleKey(new KeyValue(Mps000502ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.AgeCaption(rdo.ServiceReq.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000502ExtendSingleKey.STR_YEAR, rdo.ServiceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                    SetSingleKey(new KeyValue(Mps000502ExtendSingleKey.STR_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.ServiceReq.TDL_PATIENT_DOB)));
                }
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.Treatment);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReq);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void SetListData()
        //{
        //    try
        //    {
        //        if (rdo.lstSereNmse != null && rdo.lstSereNmse.Count > 0)
        //        {
        //            foreach (var item in rdo.lstSereNmse)
        //            {
        //                var ado = new SereNmseAdo(item);
        //                listAdo.Add(ado);
        //            }
        //            listAdo = listAdo.OrderBy(o => o.TDL_SERVICE_CODE).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
    }
}
