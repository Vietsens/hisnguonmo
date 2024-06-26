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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;

using MPS.Processor.Mps000442.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace MPS.Processor.Mps000442
{
    public class Mps000442Processor : AbstractProcessor
    {
        Mps000442.PDO.Mps000442PDO rdo;
        // List<Mps000442PDO> _ListAdo = null;
        List<SereServADO> _SereServADOs { get; set; }
        public Mps000442Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000442PDO)rdoBase;
            //_ListAdo = new List<Mps000442PDO>(); 
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                if (rdo.vaccination == null)
                {
                    rdo.vaccination = new V_HIS_VACCINATION();
                }
                SetSingleKey();
                //ProcessSereServADO();
                SetBarcodeKey();
                // var exp = BackendDataWorker.Get<HIS_EXP_MEST>().First(o => o.VACCINATION_ID == rdo.vaccination.ID);
                if (rdo.ListhisExpMestMedi == null || rdo.ListhisExpMestMedi.Count < 1)
                {
                    rdo.ListhisExpMestMedi = new List<V_HIS_EXP_MEST_MEDICINE>();
                }
               
                objectTag.AddObjectData(store, "Medicines", rdo.ListhisExpMestMedi);
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

      
        private void ProcessSereServADO()
        {
            try
            {
                _SereServADOs = new List<SereServADO>();
                if (rdo.Listexpmest != null && rdo.Listexpmest.Count > 0)
                {
                    var dataSereServ = rdo.Listexpmest.ToList();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataSereServ), dataSereServ));
                    if (dataSereServ != null && dataSereServ.Count > 0)
                    {
                        foreach (var item in dataSereServ)
                        {
                            var dataPatientTypeName = BackendDataWorker.Get<HIS_EXP_MEST>().Where(o => o.ID == item.VACCINATION_ID);
                            SereServADO ado = new SereServADO(item);

                            _SereServADOs.Add(ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo.vaccination != null)
                {
                    if (!String.IsNullOrEmpty(rdo.vaccination.VACCINATION_EXAM_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.vaccination.VACCINATION_EXAM_CODE);
                        barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatientCode.IncludeLabel = false;
                        barcodePatientCode.Width = 120;
                        barcodePatientCode.Height = 40;
                        barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodePatientCode.IncludeLabel = true;

                        dicImage.Add(Mps000442ExtendSingleKey.VACCINATION_EXAM_CODE_BAR, barcodePatientCode);
                    }
                    
                    if (!String.IsNullOrEmpty(rdo.vaccination.VACCINATION_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.vaccination.VACCINATION_CODE);
                        barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment.IncludeLabel = false;
                        barcodeTreatment.Width = 120;
                        barcodeTreatment.Height = 40;
                        barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment.IncludeLabel = true;

                        dicImage.Add(Mps000442ExtendSingleKey.VACCINATION_CODE_BAR, barcodeTreatment);
                    }
                    
                    if (!String.IsNullOrEmpty(rdo.vaccination.EXP_MEST_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeServiceReq = new Inventec.Common.BarcodeLib.Barcode(rdo.vaccination.EXP_MEST_CODE);
                        barcodeServiceReq.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeServiceReq.IncludeLabel = false;
                        barcodeServiceReq.Width = 120;
                        barcodeServiceReq.Height = 40;
                        barcodeServiceReq.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeServiceReq.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeServiceReq.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeServiceReq.IncludeLabel = true;
                        //rdo.vaccination.EXP_MEST_CODE_BAR
                        dicImage.Add(Mps000442ExtendSingleKey.EXP_MEST_CODE_BAR, barcodeServiceReq);
                    }
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
                //if (rdo.vaccination != null)
                //{
                //    if (rdo.vaccination != null && rdo.vaccination.Cou > 0)
                //    {
                //        //sắp xếp theo thứ tự tăng dần của id
                //        rdo.vaccination = rdo.vaccination.OrderBy(o => o.ID).ToList();
                //        foreach (var item in rdo.vaccination)
                //        {
                //            _ListAdo.Add(new Mps000442PDO(item));
                //            if (!item.PRICE.HasValue)
                //                continue;
                //            sumPrice += item.AMOUNT * item.PRICE.Value * (1 + (item.VAT_RATIO ?? 0));
                //        }
                //    }
                SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.TDL_PATIENT_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vaccination.TDL_PATIENT_DOB)));
                SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.TDL_PATIENT_NAME, rdo.vaccination.TDL_PATIENT_NAME));
               // SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.TDL_PATIENT_DOB, rdo.vaccination.TDL_PATIENT_DOB));
                SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.TDL_PATIENT_GENDER_NAME, rdo.vaccination.TDL_PATIENT_GENDER_NAME));

                SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.TDL_PATIENT_ADDRESS, rdo.vaccination.TDL_PATIENT_ADDRESS));
                SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.REQUEST_ROOM_NAME, rdo.vaccination.REQUEST_ROOM_NAME));
                SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.EXECUTE_ROOM_NAME, rdo.vaccination.EXECUTE_ROOM_NAME));
                SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.VACCINATION_EXAM_CODE, rdo.vaccination.VACCINATION_EXAM_CODE));
              //  SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.VACCINATION_EXAM_CODE_BAR, rdo.vaccination.VACCINATION_EXAM_CODE_BAR));
                SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.VACCINATION_CODE, rdo.vaccination.VACCINATION_CODE));
             //   SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.VACCINATION_CODE_BAR, rdo.vaccination.VACCINATION_CODE_BAR));
                SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.EXP_MEST_CODE, rdo.vaccination.EXP_MEST_CODE));
            //    SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.EXP_MEST_CODE_BAR, rdo.vaccination.EXP_MEST_CODE_BAR));
                SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.REQUEST_USERNAME, rdo.vaccination.REQUEST_USERNAME));



                //var reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().First(o => o.ID == rdo.vaccination.REQUEST_ROOM_ID);

                //if (reqRoom != null)
                //{
                //    Inventec.Common.Logging.LogSystem.Debug("ROOM_CODE" + reqRoom.ROOM_CODE);
                //    Inventec.Common.Logging.LogSystem.Debug("ROOM_NAME" + reqRoom.ROOM_NAME);
                //    SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.ROOM_CODE, reqRoom.ROOM_CODE));
                //    SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.ROOM_NAME, reqRoom.ROOM_NAME));
                //}
                //  SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.VACCINATION_EXAM_CODE, rdo.vaccination.VACCINATION_EXAM_CODE));

                //  SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.REQUEST_USERNAME, rdo.vaccination.REQUEST_USERNAME));

                //var exp = BackendDataWorker.Get<HIS_EXP_MEST>().First(o => o.VACCINATION_ID == rdo.vaccination.ID);
                //if (exp != null)
                //{
                //    SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.AMOUNT, Inventec.Common.Number.Convert.NumberToString(exp.AMOUNT)));
                //    SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.MEDICINE_TYPE_NAME, exp.MEDICINE_TYPE_NAME));
                //    SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.SERVICE_UNIT_NAME, exp.SERVICE_UNIT_NAME));
                //}
                SetSingleKey(new KeyValue(Mps000442ExtendSingleKey.DOB, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.vaccination.TDL_PATIENT_DOB)));
                AddObjectKeyIntoListkey<V_HIS_VACCINATION>(rdo.vaccination);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    
}
