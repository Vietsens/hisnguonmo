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
using AutoMapper;
using FlexCel.Report;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000101.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MPS.Processor.Mps000101
{
    class Mps000101Processor : AbstractProcessor
    {
        Mps000101PDO rdo;
        List<Mps000101ADO> ImpMestManuMedicineSumForPrints = new List<Mps000101ADO>();
        List<ImpMestADO> ImpMestADOs = new List<ImpMestADO>();

        public Mps000101Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000101PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.AggrImpMest.IMP_MEST_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000101ExtendSingleKey.IMP_MEST_CODE_BAR, barcodePatientCode);

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
                ProcessListADO();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                ProcessSingleKey();
                SetBarcodeKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                if (ImpMestADOs != null && ImpMestADOs.Count > 0)
                {
                    ImpMestADOs = ImpMestADOs.OrderBy(o => o.TDL_PATIENT_FIRST_NAME).ToList();
                }

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ImpMestAggregates", ImpMestManuMedicineSumForPrints);
                objectTag.AddObjectData(store, "ImpMests", this.ImpMestADOs);
                objectTag.AddRelationship(store, "ImpMestAggregates", "ImpMests", new string[] { "MEDICINE_TYPE_ID", "IS_MEDICINE" }, new string[] { "MEDICINE_TYPE_ID", "IS_MEDICINE" });

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void ProcessListADO()
        {
            try
            {
                ImpMestManuMedicineSumForPrints = new List<Mps000101ADO>();
                if (rdo._ImpMestMedicines != null && rdo._ImpMestMedicines.Count > 0)
                {
                    List<V_HIS_IMP_MEST_MEDICINE> createDatas = new List<V_HIS_IMP_MEST_MEDICINE>();
                    createDatas = rdo._ImpMestMedicines.Where(p => Check(p)).ToList();
                    if (rdo.RoomIds != null && rdo.RoomIds.Count > 0)
                    {
                        createDatas = rdo._ImpMestMedicines.Where(o => rdo.RoomIds.Contains(o.REQ_ROOM_ID ?? 0)).ToList();
                    }
                    var dataGroups = createDatas.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                    ImpMestManuMedicineSumForPrints.AddRange((from r in dataGroups select new Mps000101ADO(r, rdo.AggrImpMest.IMP_MEST_STT_ID, rdo.HisImpMestSttId__Imported, rdo.HisImpMestSttId__Approved)).ToList());

                    foreach (var item in rdo._ImpMestMedicines)
                    {
                        var imp = rdo._ImpMests_Print.FirstOrDefault(o => o.ID == item.IMP_MEST_ID);
                        if (imp != null)
                        {
                            ImpMestADO ado = new ImpMestADO(item, rdo.AggrImpMest.IMP_MEST_STT_ID, rdo.HisImpMestSttId__Imported, rdo.HisImpMestSttId__Approved, imp);
                            ImpMestADOs.Add(ado);
                        }
                    }
                }

                if (ImpMestManuMedicineSumForPrints != null && ImpMestManuMedicineSumForPrints.Count > 0)
                {
                    ImpMestManuMedicineSumForPrints = ImpMestManuMedicineSumForPrints.OrderBy(p => p.MEDICINE_TYPE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool Check(V_HIS_IMP_MEST_MEDICINE _medi)
        {
            bool result = false;
            try
            {
                var data = rdo.vHisMedicineType.FirstOrDefault(p => p.ID == _medi.MEDICINE_TYPE_ID);
                if (data != null)
                {
                    if (rdo.ServiceUnitIds != null
                        && rdo.ServiceUnitIds.Count > 0)
                    {
                        if (rdo.ServiceUnitIds.Contains(data.SERVICE_UNIT_ID))
                            result = true;
                    }
                    if (data.MEDICINE_USE_FORM_ID > 0)
                    {
                        if (rdo.UseFormIds != null
                    && rdo.UseFormIds.Count > 0 && rdo.UseFormIds.Contains(data.MEDICINE_USE_FORM_ID ?? 0))
                        {
                            result = result && true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        void ProcessSingleKey()
        {
            try
            {
                if (rdo._MobaExpMests != null && rdo._MobaExpMests.Count > 0)
                {
                    var minTime = rdo._MobaExpMests.Min(p => p.TDL_INTRUCTION_TIME ?? 0);
                    var maxTime = rdo._MobaExpMests.Max(p => p.TDL_INTRUCTION_TIME ?? 0);
                    SetSingleKey(new KeyValue(Mps000101ExtendSingleKey.MIN_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minTime)));
                    SetSingleKey(new KeyValue(Mps000101ExtendSingleKey.MIN_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(minTime)));
                    SetSingleKey(new KeyValue(Mps000101ExtendSingleKey.MAX_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000101ExtendSingleKey.MAX_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxTime)));
                }
                string name = "";
                switch (rdo.keyNameTitles)
                {
                    case IsTittle101.GayNghienTamThan:
                        name = "GÂY NGHIỆN, HƯỚNG THẦN";
                        break;
                    case IsTittle101.GayNghien:
                        name = "GÂY NGHIỆN";
                        break;
                    case IsTittle101.TamThan:
                        name = "HƯỚNG THẦN";
                        break;
                    case IsTittle101.ThuocDoc:
                        name = "ĐỘC";
                        break;
                    default:
                        break;
                }
                SetSingleKey(new KeyValue(Mps000101ExtendSingleKey.KEY_NAME_TITLES, name));

                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(rdo.AggrImpMest, false);
                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(rdo.Department, false);

                SetSingleKey(new KeyValue(Mps000101ExtendSingleKey.REQ_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.AggrImpMest.CREATE_TIME ?? 0)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                if (rdo != null && rdo.AggrImpMest != null)
                {
                    log = LogDataImpMest("", rdo.AggrImpMest.IMP_MEST_CODE, rdo.Department.DEPARTMENT_NAME);
                }
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                string name = "";
                switch (rdo.keyNameTitles)
                {
                    case IsTittle101.GayNghienTamThan:
                        name = "GN_HT";
                        break;
                    case IsTittle101.GayNghien:
                        name = "GN";
                        break;
                    case IsTittle101.TamThan:
                        name = "HT";
                        break;
                    case IsTittle101.ThuocDoc:
                        name = "Đ";
                        break;
                    default:
                        break;
                }

                if (rdo != null && rdo.AggrImpMest != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}", printTypeCode, rdo.AggrImpMest.IMP_MEST_CODE, rdo.AggrImpMest.MEDI_STOCK_CODE, rdo.Department.DEPARTMENT_CODE, name);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
