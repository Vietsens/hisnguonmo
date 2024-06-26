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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000175.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000175
{
    class Mps000175Processor : AbstractProcessor
    {
        Mps000175PDO rdo;

        public Mps000175Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000175PDO)rdoBase;
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

                dicImage.Add(Mps000175ExtendSingleKey.EXP_MEST_CODE_BARCODE, barcodePatientCode);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetSingleKey()
        {
            try
            {
                if (rdo._ExpMests_Print != null && rdo._ExpMests_Print.Count > 0)
                {
                    var minTime = rdo._ExpMests_Print.Where(p => p.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT).Min(p => p.TDL_INTRUCTION_TIME ?? 0);
                    var maxTime = rdo._ExpMests_Print.Where(p => p.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT).Max(p => p.TDL_INTRUCTION_TIME ?? 0);
                    SetSingleKey(new KeyValue(Mps000175ExtendSingleKey.MIN_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minTime)));
                    SetSingleKey(new KeyValue(Mps000175ExtendSingleKey.MIN_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(minTime)));
                    SetSingleKey(new KeyValue(Mps000175ExtendSingleKey.MIN_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(minTime)));

                    SetSingleKey(new KeyValue(Mps000175ExtendSingleKey.MAX_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000175ExtendSingleKey.MAX_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000175ExtendSingleKey.MAX_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(maxTime)));

                }

                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>(rdo.AggrExpMest, false);
                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(rdo.Department, false);

                //SetSingleKey(new KeyValue(Mps000175ExtendSingleKey.KEY_NAME_TITLES, rdo.title));
                switch (rdo.keyName)
                {
                    case keyTitles.phieuLinhVatTu:
                        SetSingleKey(new KeyValue(Mps000175ExtendSingleKey.KEY_NAME_TITLES, "VẬT TƯ"));
                        break;
                    case keyTitles.phieuLinhHoaChat:
                        SetSingleKey(new KeyValue(Mps000175ExtendSingleKey.KEY_NAME_TITLES, "HÓA CHẤT"));
                        break;
                    default:
                        break;
                }

                SetSingleKey(new KeyValue(Mps000175ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000175ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000175ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.AggrExpMest.CREATE_TIME ?? 0)));

                if (this.rdo.ListAggrExpMest != null && this.rdo.ListAggrExpMest.Count > 0)
                {
                    string expMestCodes = string.Join(",", rdo.ListAggrExpMest.Select(s => s.EXP_MEST_CODE).Distinct().ToList());
                    SetSingleKey((new KeyValue(Mps000175ExtendSingleKey.EXP_MEST_CODEs, expMestCodes)));

                    List<string> createTime = new List<string>();
                    var list = this.rdo._ExpMestMaterials.Select(o => o.AGGR_EXP_MEST_ID).ToList();
                    this.rdo.ListAggrExpMest = this.rdo.ListAggrExpMest.OrderBy(o => o.CREATE_TIME).Where(o => o.MEDI_STOCK_CODE == rdo.AggrExpMest.MEDI_STOCK_CODE).ToList();
                    foreach (var item in this.rdo.ListAggrExpMest)
                    {
                        if (list != null && list.Count > 0 && list.Contains(item.ID))
                        {
                            createTime.Add(Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.CREATE_DATE));
                        }
                    }
                    createTime = createTime.Distinct().ToList();
                    var createTimeStr = String.Join(",", createTime);
                    SetSingleKey(new KeyValue(Mps000175ExtendSingleKey.CREATE_TIME_AGGR_STR, createTimeStr));
                }

                SetSingleKey(new KeyValue(Mps000175ExtendSingleKey.TOTAL_REQ_ROOM_NAME_DISPLAY, totalReqRoomName));
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string totalReqRoomName = "";
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

                SetBarcodeKey();
                SetSingleKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
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

        void ProcessListADO()
        {
            try
            {
                List<long> reqRoomIds = new List<long>();

                if (rdo._ExpMestMaterials != null && rdo._ExpMestMaterials.Count > 0)
                {
                    var query = rdo._ExpMestMaterials.ToList();
                    if (rdo.RoomIds != null && rdo.RoomIds.Count > 0 && rdo._ExpMests_Print != null && rdo._ExpMests_Print.Count > 0)
                    {
                        query = query.Where(p => rdo.RoomIds.Contains(p.REQ_ROOM_ID)).ToList();
                    }
                    query = query.Where(p => Check(p)).ToList();

                    if (query != null && query.Count > 0)
                    {
                        reqRoomIds.AddRange(query.Select(s => s.REQ_ROOM_ID).ToList());
                    }

                    //Review
                    #region ------Code Cu ------
                    if (rdo.ConfigMps175._ConfigKeyMERGER_DATA == 1)
                    {
                        var Groups = query.GroupBy(g => new
                        {
                            g.MATERIAL_ID,
                        }).Select(p => p.ToList()).ToList();
                        rdo.listAdo.AddRange(from r in Groups
                                             select new Mps000175ADO(1, rdo.AggrExpMest,
                                                 r,
                                                 rdo.ConfigMps175._ExpMestSttId__Approved,
                                                 rdo.ConfigMps175._ExpMestSttId__Exported,
                                                 rdo.ConfigMps175.PatientTypeId__BHYT,
                                                 rdo._MaterialTypes));
                    }
                    else
                    {
                        var Groups = query.GroupBy(g => new
                        {
                            g.MATERIAL_TYPE_ID
                        }).Select(p => p.ToList()).ToList();
                        rdo.listAdo.AddRange(from r in Groups
                                             select new Mps000175ADO(rdo.AggrExpMest,
                                                 r,
                                                 rdo.ConfigMps175._ExpMestSttId__Approved,
                                                 rdo.ConfigMps175._ExpMestSttId__Exported,
                                                 rdo.ConfigMps175.PatientTypeId__BHYT,
                                                 rdo._MaterialTypes));
                    }
                    #endregion
                }

                if (rdo.listAdo != null && rdo.listAdo.Count > 0)
                {
                    rdo.listAdo = rdo.listAdo.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                    SetSingleKey(new KeyValue(Mps000175ExtendSingleKey.TOTAL_PRICE, rdo.listAdo.Sum(o => o.PRICE ?? 0)));
                    SetSingleKey(new KeyValue(Mps000175ExtendSingleKey.TOTAL_PRICE_VAT, rdo.listAdo.Sum(o => (o.PRICE ?? 0) * (1 + (o.VAT_RATIO ?? 0)))));
                }

                if (reqRoomIds.Count > 0)
                {
                    var reqrooms = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => reqRoomIds.Contains(o.ID)).ToList();
                    if (reqrooms != null && reqrooms.Count > 0)
                    {
                        totalReqRoomName = String.Join("; ", reqrooms.Select(s => s.ROOM_NAME).OrderBy(o => o));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool Check(V_HIS_EXP_MEST_MATERIAL _expMestMaterial)
        {
            bool result = false;
            try
            {
                if (_expMestMaterial != null)
                {
                    if (rdo.ServiceUnitIds != null && rdo.ServiceUnitIds.Count > 0)
                    {
                        if (rdo.ServiceUnitIds.Contains(_expMestMaterial.SERVICE_UNIT_ID))
                            result = true;
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
                if (rdo != null && rdo.AggrExpMest != null && rdo._ExpMests_Print != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}", printTypeCode, rdo.AggrExpMest.EXP_MEST_CODE, rdo.AggrExpMest.MEDI_STOCK_CODE, rdo._ExpMests_Print.Count(), "Phiếu lĩnh", rdo.listAdo.FirstOrDefault().MEDICINE_TYPE_CODE, rdo.listAdo.Count());
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
