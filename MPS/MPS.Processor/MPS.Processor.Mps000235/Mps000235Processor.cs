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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000235.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000235
{
    public class Mps000235Processor : AbstractProcessor
    {
        Mps000235PDO rdo;

        List<HIS_PATIENT_TYPE_ALTER> _PATIENT_TYPE_ALTERs = null;

        long? SoThang;

        public Mps000235Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000235PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.AggrExpMest.EXP_MEST_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000235ExtendSingleKey.EXP_MEST_CODE_BAR, barcodePatientCode);
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

                //Dangth

                objectTag.AddObjectData(store, "PatientTypeAlter", _PATIENT_TYPE_ALTERs);              
                objectTag.AddObjectData(store, "ExpMestAggregates", rdo.listAdo);
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
                if (rdo._ExpMests_Print != null && rdo._ExpMests_Print.Count > 0)
                {
                    var minTime = rdo._ExpMests_Print.Min(p => p.TDL_INTRUCTION_TIME ?? 0);
                    var maxTime = rdo._ExpMests_Print.Max(p => p.TDL_INTRUCTION_TIME ?? 0);
                    SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.MIN_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minTime)));
                    SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.MIN_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(minTime)));
                    SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.MIN_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(minTime)));

                    SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.MAX_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.MAX_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.MAX_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(maxTime)));
                }
                if (rdo._BedRoom != null)
                {
                    SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.BED_ROOM_NAME, rdo._BedRoom.BED_ROOM_NAME));
                    SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.BED_NAME, rdo._BedRoom.BED_NAME));
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
                SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.KEY_NAME_TITLES, keyNameTitles));

                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(rdo.Department, false);
                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>(rdo.AggrExpMest, false);
                //SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.REQ_LOGINNAME, rdo.AggrExpMest.REQ_LOGINNAME));
                //SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.REQ_USERNAME, rdo.AggrExpMest.REQ_USERNAME));
                SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.AggrExpMest.CREATE_TIME ?? 0)));

                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo._Patient, true);

                if (SoThang <= 0) SoThang = null;
                SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.REMEDY_COUNT, SoThang));

                if (rdo._Treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.ICD_CODE, rdo._Treatment.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.ICD_NAME, rdo._Treatment.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.ICD_SUB_CODE, rdo._Treatment.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000235ExtendSingleKey.ICD_TEXT, rdo._Treatment.ICD_TEXT));
                }

                string totalMediStockName = "";
                if (rdo.ListMediStock != null && rdo.ListMediStock.Count > 0 && rdo._ExpMests_Print != null && rdo._ExpMests_Print.Count > 0)
                {
                    var listMediStock = rdo.ListMediStock.Where(o => rdo._ExpMests_Print.Exists(e => e.MEDI_STOCK_ID == o.ID)).ToList();
                    if (listMediStock != null && listMediStock.Count > 0)
                    {
                        var parentStock = rdo.ListMediStock.Where(o => listMediStock.Exists(e => e.PARENT_ID == o.ID)).ToList();
                        if (parentStock != null && parentStock.Count > 0)
                        {
                            totalMediStockName = parentStock.First().MEDI_STOCK_NAME;
                        }
                        else
                        {
                            totalMediStockName = string.Join("; ", listMediStock.Select(s => s.MEDI_STOCK_NAME).OrderBy(o => o).Distinct());
                        }
                    }
                }

                SetSingleKey(new KeyValue("TOTAL_MEDI_STOCK_NAME", totalMediStockName));
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
                if (rdo._ExpMests_Print == null) return;

                List<long> _ExpMestIds = rdo._ExpMests_Print.Where(p => p.TDL_PATIENT_ID == rdo._Patient.ID).Select(p => p.ID).ToList();
                if (_ExpMestIds != null && _ExpMestIds.Count > 0)
                {
                    if (rdo._ExpMestMedicines != null && rdo._ExpMestMedicines.Count > 0)
                    {
                        var query = rdo._ExpMestMedicines.Where(p => _ExpMestIds.Contains(p.EXP_MEST_ID ?? 0)).ToList();

                        List<Mps000235ADO> dataMedis = new List<Mps000235ADO>();
                        if (rdo._ConfigKeyMERGER_DATA == 1)
                        {
                            var Groups = query.GroupBy(g => new
                        {
                            g.MEDICINE_ID,
                            g.CONCENTRA
                        }).Select(p => p.ToList()).ToList();
                            dataMedis.AddRange(from r in Groups
                                               select new Mps000235ADO(rdo.AggrExpMest,
                                                   r,
                                                   rdo._ExpMestSttId__Approved,
                                                   rdo._ExpMestSttId__Exported,
                                                   rdo.ServiceReq_Remedy));
                        }
                        else
                        {
                            var Groups = query.GroupBy(g => new
                            {
                                g.MEDICINE_TYPE_ID,
                                g.CONCENTRA
                            }).Select(p => p.ToList()).ToList();
                            dataMedis.AddRange(from r in Groups
                                               select new Mps000235ADO(rdo.AggrExpMest,
                                                   r,
                                                   rdo._ExpMestSttId__Approved,
                                                   rdo._ExpMestSttId__Exported,
                                                   rdo.ServiceReq_Remedy));
                        }

                        if (dataMedis != null && dataMedis.Count > 0)
                        {
                            dataMedis = dataMedis.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                            rdo.listAdo.AddRange(dataMedis);
                        }
                    }

                    //TODO VT
                    if (rdo._ExpMestMaterials != null && rdo._ExpMestMaterials.Count > 0)
                    {
                        //List<long> _ExpMestIds = rdo._ExpMests_Print.Where(p => p.TDL_PATIENT_ID == rdo._Patient.ID).Select(p => p.ID).ToList();
                        var query = rdo._ExpMestMaterials.Where(p => _ExpMestIds.Contains(p.EXP_MEST_ID ?? 0)).ToList();

                        List<Mps000235ADO> dataMates = new List<Mps000235ADO>();
                        if (rdo._ConfigKeyMERGER_DATA == 1)
                        {
                            var Groups = query.GroupBy(g => new
                        {
                            g.MATERIAL_ID,
                            g.IS_CHEMICAL_SUBSTANCE
                        }).Select(p => p.ToList()).ToList();
                            dataMates.AddRange(from r in Groups
                                               select new Mps000235ADO(rdo.AggrExpMest,
                                                   r,
                                                   rdo._ExpMestSttId__Approved,
                                                   rdo._ExpMestSttId__Exported,
                                                   rdo.ServiceReq_Remedy));
                        }
                        else
                        {
                            var Groups = query.GroupBy(g => new
                            {
                                g.MATERIAL_TYPE_ID,
                                g.IS_CHEMICAL_SUBSTANCE
                            }).Select(p => p.ToList()).ToList();
                            dataMates.AddRange(from r in Groups
                                               select new Mps000235ADO(rdo.AggrExpMest,
                                                   r,
                                                   rdo._ExpMestSttId__Approved,
                                                   rdo._ExpMestSttId__Exported,
                                                   rdo.ServiceReq_Remedy));
                        }

                        if (dataMates != null && dataMates.Count > 0)
                        {
                            dataMates = dataMates.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                            rdo.listAdo.AddRange(dataMates);
                        }
                    }

                    if (rdo.ServiceReq_Remedy != null && rdo.ServiceReq_Remedy.Count > 0)
                    {
                        var reqIds = rdo._ExpMests_Print.Where(p => p.TDL_PATIENT_ID == rdo._Patient.ID).Select(s => s.SERVICE_REQ_ID ?? 0).Distinct().ToList();
                        var lstServiceReq = rdo.ServiceReq_Remedy.Where(p => reqIds.Contains(p.ID)).ToList();
                        if (lstServiceReq != null && lstServiceReq.Count > 0)
                        {
                            SoThang = lstServiceReq.Sum(s => s.REMEDY_COUNT ?? 0);
                        }
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
                if (rdo != null && rdo._ExpMests_Print != null)
                {
                    List<string> treatmentCodes = rdo._ExpMests_Print.Select(s => s.TDL_TREATMENT_CODE).Distinct().ToList();
                    List<string> expMestCodes = rdo._ExpMests_Print.Select(s => s.EXP_MEST_CODE).Distinct().ToList();
                    log = LogDataExpMest(string.Join(";", treatmentCodes), string.Join(";", expMestCodes), "");
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
                string keyNameTitles = "TITLES:";
                switch (rdo.keyName)
                {
                    case keyTitles.phieuLinhTongHop:
                        keyNameTitles += "TongHop";
                        break;
                    case keyTitles.phieuLinhThuocThuong:
                        keyNameTitles += "ThuocThuong";
                        break;
                    case keyTitles.GayNghienHuongThan:
                        keyNameTitles += "ThuocGayNghienThuocHuongThan";
                        break;
                    case keyTitles.GayNghien:
                        keyNameTitles += "ThuocGayNghien";
                        break;
                    case keyTitles.HuongThan:
                        keyNameTitles += "ThuocHuongThan";
                        break;
                    case keyTitles.VatTu:
                        keyNameTitles += "VatTu";
                        break;
                    case keyTitles.ThuocDoc:
                        keyNameTitles += "ThuocDoc";
                        break;
                    case keyTitles.PhongXa:
                        keyNameTitles += "ThuocPhongXa";
                        break;
                    case keyTitles.Corticoid:
                        keyNameTitles += "ThuocCorticoid";
                        break;
                    case keyTitles.DichTruyen:
                        keyNameTitles += "ThuocDichTruyen";
                        break;
                    case keyTitles.KhangSinh:
                        keyNameTitles += "ThuocKhangSinh";
                        break;
                    case keyTitles.Lao:
                        keyNameTitles += "ThuocLao";
                        break;
                    default:
                        break;
                }

                string expMestCode = "EXP_MEST_CODE:";
                string treatmentCode = "TREATMENT_CODE:";
                string dataPlus = "DATA_PLUS:";

                if (rdo != null && rdo.AggrExpMest != null)
                    expMestCode += rdo.AggrExpMest.EXP_MEST_CODE;

                if (rdo != null && rdo._Treatment != null)
                    treatmentCode += rdo._Treatment.TREATMENT_CODE;

                if (rdo != null && rdo.listAdo != null)
                    dataPlus += string.Format("{0}_{1}_{2}", (rdo._ExpMests_Print != null ? rdo._ExpMests_Print.Count : 0), rdo.listAdo.Count, rdo.listAdo.First().MEDICINE_TYPE_CODE);

                result = String.Format("{0} {1} {2} {3} {4}", printTypeCode, expMestCode, keyNameTitles, treatmentCode, dataPlus);
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
