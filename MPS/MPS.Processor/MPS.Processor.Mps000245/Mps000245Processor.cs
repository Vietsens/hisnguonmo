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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000245.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000245
{
    public class Mps000245Processor : AbstractProcessor
    {
        Mps000245PDO rdo;

        List<HIS_PATIENT_TYPE_ALTER> _PATIENT_TYPE_ALTERs = null;

        public Mps000245Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000245PDO)rdoBase;
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

                dicImage.Add(Mps000245ExtendSingleKey.IMP_MEST_CODE_BAR, barcodePatientCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                ProcessListADO();

                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                SetBarcodeKey();
                SetSingleKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "PatientTypeAlter", _PATIENT_TYPE_ALTERs);
                objectTag.AddObjectData(store, "ImpMestAggregates", rdo.listAdo);
                objectTag.SetUserFunction(store, "FuncMergeData11", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData12", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData13", new CalculateMergerData());

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void SetSingleKey()
        {
            try
            {
                if (rdo._MobaExpMests != null && rdo._MobaExpMests.Count > 0)
                {
                    var minTime = rdo._MobaExpMests.Min(p => p.TDL_INTRUCTION_TIME ?? 0);
                    var maxTime = rdo._MobaExpMests.Max(p => p.TDL_INTRUCTION_TIME ?? 0);
                    SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.MIN_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minTime)));
                    SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.MIN_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(minTime)));
                    SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.MIN_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(minTime)));

                    SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.MAX_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.MAX_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.MAX_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(maxTime)));
                }
                string keyNameTitles = "";
                switch (rdo.keyName)
                {
                    case keyTitles.phieuLinhTongHop:
                        keyNameTitles = "TỔNG HỢP";
                        break;
                    case keyTitles.phieuLinhThuocThuong:
                        keyNameTitles = "THUỐC THƯỜNG";
                        break;
                    case keyTitles.GayNghienHuongThan:
                        keyNameTitles = "THUỐC GÂY NGHIỆN, THUỐC HƯỚNG THẦN";
                        break;
                    case keyTitles.GayNghien:
                        keyNameTitles = "THUỐC GÂY NGHIỆN";
                        break;
                    case keyTitles.HuongThan:
                        keyNameTitles = "THUỐC HƯỚNG THẦN";
                        break;
                    case keyTitles.VatTu:
                        keyNameTitles = "VẬT TƯ";
                        break;
                    case keyTitles.ThuocDoc:
                        keyNameTitles = "THUỐC ĐỘC";
                        break;
                    case keyTitles.PhongXa:
                        keyNameTitles = "THUỐC PHÓNG XẠ";
                        break;
                    case keyTitles.Corticoid:
                        keyNameTitles = "THUỐC CORTICOID";
                        break;
                    case keyTitles.DichTruyen:
                        keyNameTitles = "THUỐC DỊCH TRUYỀN";
                        break;
                    case keyTitles.KhangSinh:
                        keyNameTitles = "THUỐC KHÁNG SINH";
                        break;
                    case keyTitles.Lao:
                        keyNameTitles = "THUỐC LAO";
                        break;
                    case keyTitles.TienChat:
                        keyNameTitles = "TIỀN CHẤT";
                        break;
                    default:
                        break;
                }
                SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.KEY_NAME_TITLES, keyNameTitles));

                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(rdo.Department, false);
                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(rdo.AggrImpMest, false);
                //SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.REQ_LOGINNAME, rdo.AggrExpMest.REQ_LOGINNAME));
                //SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.REQ_USERNAME, rdo.AggrExpMest.REQ_USERNAME));
                SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(rdo.AggrImpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.AggrImpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.AggrImpMest.CREATE_TIME ?? 0)));

                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo._Patient, true);
                if (rdo._Treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.ICD_CODE, rdo._Treatment.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.ICD_NAME, rdo._Treatment.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.ICD_SUB_CODE, rdo._Treatment.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.ICD_TEXT, rdo._Treatment.ICD_TEXT));
                }

                if (rdo.listAdo != null)
                {
                    SetSingleKey(new KeyValue(Mps000245ExtendSingleKey.REMEDY_COUNT, rdo.listAdo.Count));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessListADO()
        {
            try
            {
                _PATIENT_TYPE_ALTERs = new List<HIS_PATIENT_TYPE_ALTER>();
                if (rdo._PatientTYpeAlters != null && rdo._PatientTYpeAlters.Count > 0)
                {
                    var _DataPatientTypes = rdo._PatientTYpeAlters.Where(p => p.TDL_PATIENT_ID == rdo._Patient.ID).ToList();
                    if (_DataPatientTypes != null && _DataPatientTypes.Count > 0)
                    {
                        var dataGr = _DataPatientTypes.GroupBy(p => p.HEIN_CARD_NUMBER).Select(p => p.ToList()).ToList();
                        foreach (var item in dataGr)
                        {
                            _PATIENT_TYPE_ALTERs.Add(item.FirstOrDefault());
                        }
                    }
                }
                if (rdo._ImpMestMedicines != null && rdo._ImpMestMedicines.Count > 0)
                {
                    List<long> _ImpMestIds = rdo._ImpMests_Print.Where(p => p.TDL_PATIENT_ID == rdo._Patient.ID).Select(p => p.ID).ToList();
                    var query = rdo._ImpMestMedicines.Where(p => _ImpMestIds.Contains(p.IMP_MEST_ID)).ToList();

                    List<Mps000245ADO> dataMedis = new List<Mps000245ADO>();
                    if (rdo._ConfigKeyMERGER_DATA == 1)
                    {
                        var Groups = query.GroupBy(g => new
                    {
                        g.MEDICINE_ID,
                        g.CONCENTRA
                    }).Select(p => p.ToList()).ToList();
                        dataMedis.AddRange(from r in Groups
                                           select new Mps000245ADO(rdo.AggrImpMest,
                                               r,
                                               rdo._ImpMestSttId__Approved,
                                               rdo._ImpMestSttId__Imported));
                    }
                    else
                    {
                        var Groups = query.GroupBy(g => new
                        {
                            g.MEDICINE_TYPE_ID,
                            g.CONCENTRA
                        }).Select(p => p.ToList()).ToList();
                        dataMedis.AddRange(from r in Groups
                                           select new Mps000245ADO(rdo.AggrImpMest,
                                               r,
                                               rdo._ImpMestSttId__Approved,
                                               rdo._ImpMestSttId__Imported));
                    }
                    if (dataMedis != null && dataMedis.Count > 0)
                    {
                        dataMedis = dataMedis.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDI_MATE_TYPE_NAME).ToList();
                        rdo.listAdo.AddRange(dataMedis);
                    }
                }

                //TODO VT
                if (rdo._ImpMestMaterials != null && rdo._ImpMestMaterials.Count > 0)
                {
                    List<long> _ImpMestIds = rdo._ImpMests_Print.Where(p => p.TDL_PATIENT_ID == rdo._Patient.ID).Select(p => p.ID).ToList();
                    var query = rdo._ImpMestMaterials.Where(p => _ImpMestIds.Contains(p.IMP_MEST_ID)).ToList();

                    List<Mps000245ADO> dataMates = new List<Mps000245ADO>();
                    if (rdo._ConfigKeyMERGER_DATA == 1)
                    {
                        var Groups = query.GroupBy(g => new
                    {
                        g.MATERIAL_ID,
                    }).Select(p => p.ToList()).ToList();
                        dataMates.AddRange(from r in Groups
                                           select new Mps000245ADO(rdo.AggrImpMest,
                                               r,
                                               rdo._ImpMestSttId__Approved,
                                               rdo._ImpMestSttId__Imported));
                    }
                    else
                    {
                        var Groups = query.GroupBy(g => new
                        {
                            g.MATERIAL_TYPE_ID
                        }).Select(p => p.ToList()).ToList();
                        dataMates.AddRange(from r in Groups
                                           select new Mps000245ADO(rdo.AggrImpMest,
                                               r,
                                               rdo._ImpMestSttId__Approved,
                                               rdo._ImpMestSttId__Imported));
                    }
                    if (dataMates != null && dataMates.Count > 0)
                    {
                        dataMates = dataMates.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDI_MATE_TYPE_NAME).ToList();
                        rdo.listAdo.AddRange(dataMates);
                    }
                }
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
                log = LogDataImpMest(rdo.AggrImpMest.TDL_TREATMENT_CODE, rdo.AggrImpMest.IMP_MEST_CODE, "");
                log += string.Format("Kho: {0}, Phòng yêu cầu: {1}", rdo.AggrImpMest.MEDI_STOCK_NAME, rdo.AggrImpMest.REQ_ROOM_ID);
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
                string keyNameTitles = "";
                switch (rdo.keyName)
                {
                    case keyTitles.phieuLinhTongHop:
                        keyNameTitles = "TH";
                        break;
                    case keyTitles.phieuLinhThuocThuong:
                        keyNameTitles = "TT";
                        break;
                    case keyTitles.GayNghienHuongThan:
                        keyNameTitles = "GN_HT";
                        break;
                    case keyTitles.GayNghien:
                        keyNameTitles = "GN";
                        break;
                    case keyTitles.HuongThan:
                        keyNameTitles = "HT";
                        break;
                    case keyTitles.VatTu:
                        keyNameTitles = "VT";
                        break;
                    case keyTitles.ThuocDoc:
                        keyNameTitles = "Đ";
                        break;
                    case keyTitles.PhongXa:
                        keyNameTitles = "PX";
                        break;
                    default:
                        break;
                }

                int countMedicine = 0;
                int countMaterial = 0;
                if (rdo._ImpMestMedicines != null)
                {
                    countMedicine = rdo._ImpMestMedicines.Count;
                }

                if (rdo._ImpMestMaterials != null)
                {
                    countMaterial = rdo._ImpMestMaterials.Count;
                }

                if (rdo != null && rdo.AggrImpMest != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}_{5}", printTypeCode, rdo.AggrImpMest.IMP_MEST_CODE, rdo.AggrImpMest.MEDI_STOCK_CODE, countMedicine, countMaterial, keyNameTitles);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        class CalculateMergerData : TFlexCelUserFunction
        {
            long mediMateTypeId = 0;

            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
                bool result = false;
                try
                {
                    long mediMateId = Convert.ToInt64(parameters[0]);

                    if (mediMateId > 0)
                    {
                        if (this.mediMateTypeId == mediMateId)
                        {
                            return true;
                        }
                        else
                        {
                            this.mediMateTypeId = mediMateId;
                            return false;
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
        }
    }
}
