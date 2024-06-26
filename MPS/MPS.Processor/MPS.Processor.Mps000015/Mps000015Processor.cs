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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000015.PDO;
using MPS.ProcessorBase.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000015
{
    public class Mps000015Processor : AbstractProcessor
    {
        Mps000015PDO rdo;
        public Mps000015Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000015PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo._ServiceReq.TDL_PATIENT_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000015ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo._ServiceReq.TDL_TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000015ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetSingleKey();
                SetBarcodeKey();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "SereServ", rdo._SereServs);
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
                if (rdo._PatyAlterBhyt != null)
                {
                    SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.HEIN_CARD_NUMBER, rdo._PatyAlterBhyt.HEIN_CARD_NUMBER));
                    SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.HEIN_MEDI_ORG_CODE, rdo._PatyAlterBhyt.HEIN_MEDI_ORG_CODE));

                    SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.IS_HEIN, "X"));
                    if (rdo._PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo._PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo._PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo._PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }
                    //Dia chi the
                    SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.HEIN_CARD_ADDRESS, rdo._PatyAlterBhyt.ADDRESS));
                }
                else
                    SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (rdo._ServiceReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.INTRUCTION_TIME_FULL_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ServiceReq.INTRUCTION_TIME)));
                    SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.INTRUCTION_DATE_FULL_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ServiceReq.INTRUCTION_TIME)));
                    if (rdo._ServiceReq.FINISH_TIME != null)
                    {
                        SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.FINISH_TIME_FULL_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ServiceReq.FINISH_TIME ?? 0)));
                        SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.FINISH_DATE_FULL_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ServiceReq.FINISH_TIME ?? 0)));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.FINISH_TIME_FULL_STR, ""));
                        SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.FINISH_DATE_FULL_STR, ""));
                    }
                    SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo._ServiceReq.TDL_PATIENT_DOB)));
                }

                if (rdo._SingleKeys != null)
                {
                    SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.RATIO, rdo._SingleKeys.Ratio));
                    SetSingleKey(new KeyValue(Mps000015ExtendSingleKey.RATIO_STR, rdo._SingleKeys.Ratio * 100 + " %"));
                }
                AddObjectKeyIntoListkey<SingleKeys>(rdo._SingleKeys, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo._PatyAlterBhyt, false);
                AddObjectKeyIntoListkey<HIS_SERVICE_REQ>(rdo._ServiceReq);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
