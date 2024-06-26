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
using MPS.Processor.Mps000046.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MPS.Processor.Mps000046
{
    class Mps000046Processor : AbstractProcessor
    {
        Mps000046PDO rdo;
        List<ExpMestADO> ExpMestADOs;
        List<ExpMestADO> ExpMestADO_Logins;
        List<Mps000046ADO> listAdo_Logins;
        List<ReqLoginADO> ReqLoginADOs;

        public Mps000046Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000046PDO)rdoBase;
            //ExpMestManuMedicineSumForPrints = new List<Mps000046ADO>();
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode expMestCodeBar = new Inventec.Common.BarcodeLib.Barcode(rdo.AggrExpMest.EXP_MEST_CODE);
                expMestCodeBar.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                expMestCodeBar.IncludeLabel = false;
                expMestCodeBar.Width = 120;
                expMestCodeBar.Height = 40;
                expMestCodeBar.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                expMestCodeBar.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                expMestCodeBar.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                expMestCodeBar.IncludeLabel = true;

                dicImage.Add(Mps000046ExtendSingleKey.EXP_MEST_CODE_BARCODE, expMestCodeBar);

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
                ProcessListADO_LOGIN();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                SetSingleKey();
                SetBarcodeKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                if (ExpMestADOs != null && ExpMestADOs.Count > 0)
                {
                    ExpMestADOs = ExpMestADOs.OrderBy(o => o.TDL_PATIENT_FIRST_NAME).ToList();
                }

                if (ExpMestADO_Logins != null && ExpMestADO_Logins.Count > 0)
                {
                    ExpMestADO_Logins = ExpMestADO_Logins.OrderBy(o => o.TDL_PATIENT_FIRST_NAME).ToList();
                }

                objectTag.AddObjectData(store, "ExpMestAggregates", rdo.listAdo);
                objectTag.AddObjectData(store, "ExpMests", this.ExpMestADOs);

                objectTag.AddObjectData(store, "ReqLogin", this.ReqLoginADOs);
                objectTag.AddObjectData(store, "ExpMestAggLogin", this.listAdo_Logins);
                objectTag.AddObjectData(store, "ExpMestLogin", this.ExpMestADO_Logins);

                objectTag.AddRelationship(store, "ExpMestAggregates", "ExpMests", new string[] { "MEDI_MATE_TYPE_ID", "TYPE_ID" }, new string[] { "MEDI_MATE_TYPE_ID", "TYPE_ID" });

                objectTag.AddRelationship(store, "ReqLogin", "ExpMestAggLogin", new string[] { "REQ_LOGINNAME", "TYPE_ID" }, new string[] { "REQ_LOGINNAME", "TYPE_ID" });
                objectTag.AddRelationship(store, "ReqLogin", "ExpMestLogin", new string[] { "REQ_LOGINNAME", "TYPE_ID" }, new string[] { "REQ_LOGINNAME", "TYPE_ID" });
                objectTag.AddRelationship(store, "ExpMestAggLogin", "ExpMestLogin", new string[] { "MEDI_MATE_TYPE_ID", "TYPE_ID" }, new string[] { "MEDI_MATE_TYPE_ID", "TYPE_ID" });

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
                    var minTime = rdo._ExpMests_Print.Where(p => p.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT).ToList().Min(p => p.TDL_INTRUCTION_TIME ?? 0);
                    var maxTime = rdo._ExpMests_Print.Where(p => p.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT).ToList().Max(p => p.TDL_INTRUCTION_TIME ?? 0);
                    SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.MIN_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minTime)));
                    SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.MIN_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(minTime)));
                    SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.MIN_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(minTime)));

                    SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.MAX_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.MAX_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.MAX_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(maxTime)));
                }
                switch (rdo.keyName)
                {
                    case keyTitles.phieuLinhTongHop:
                        SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.KEY_NAME_TITLES, "TỔNG HỢP"));
                        break;
                    case keyTitles.phieuLinhThuocThuong:
                        SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.KEY_NAME_TITLES, "THUỐC THƯỜNG"));
                        break;
                    case keyTitles.Corticoid:
                        SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.KEY_NAME_TITLES, "THUỐC CORTICOID"));
                        break;
                    case keyTitles.DichTruyen:
                        SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.KEY_NAME_TITLES, "THUỐC DỊCH TRUYỀN"));
                        break;
                    case keyTitles.KhangSinh:
                        SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.KEY_NAME_TITLES, "THUỐC KHÁNG SINH"));
                        break;
                    case keyTitles.Lao:
                        SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.KEY_NAME_TITLES, "THUỐC LAO"));
                        break;
                    case keyTitles.TienChat:
                        SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.KEY_NAME_TITLES, "THUỐC TIỀN CHẤT"));
                        break;
                    default:
                        break;
                }

                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>(rdo.AggrExpMest, false);
                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(rdo.Department, false);

                SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.AggrExpMest.CREATE_TIME ?? 0)));
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
                ExpMestADOs = new List<ExpMestADO>();
                if (rdo.IsMedicine && rdo._ExpMestMedicines != null && rdo._ExpMestMedicines.Count > 0)
                {
                    var query = rdo._ExpMestMedicines.ToList();
                    if (rdo.RoomIds != null && rdo.RoomIds.Count > 0)
                    {
                        query = query.Where(p => rdo.RoomIds.Contains(p.REQ_ROOM_ID)).ToList();
                    }
                    query = query.Where(p => Check(p)).ToList();

                    List<Mps000046ADO> dataMedis = new List<Mps000046ADO>();
                    if (rdo._ConfigKeyMERGER_DATA == 1)
                    {
                        var Groups = query.GroupBy(g => new
                    {
                        g.MEDICINE_ID,
                        g.CONCENTRA
                    }).Select(p => p.ToList()).ToList();
                        dataMedis.AddRange(from r in Groups
                                           select new Mps000046ADO(rdo.AggrExpMest,
                                               r,
                                               rdo._ExpMestSttId__Approved,
                                               rdo._ExpMestSttId__Exported));

                        foreach (var gr in Groups)
                        {
                            var exp = rdo._ExpMests_Print.Where(o => gr.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                            if (exp != null && exp.Count > 0)
                            {
                                var grexp = exp.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                foreach (var item in grexp)
                                {
                                    var lst = gr.Where(o => item.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                                    ExpMestADO ado = new ExpMestADO(rdo.AggrExpMest, lst, rdo._ExpMestSttId__Approved, rdo._ExpMestSttId__Exported, item.First());
                                    ExpMestADOs.Add(ado);
                                }
                            }
                        }
                    }
                    else
                    {
                        var Groups = query.GroupBy(g => new
                        {
                            g.MEDICINE_TYPE_ID,
                            g.CONCENTRA
                        }).Select(p => p.ToList()).ToList();
                        dataMedis.AddRange(from r in Groups
                                           select new Mps000046ADO(rdo.AggrExpMest,
                                               r,
                                               rdo._ExpMestSttId__Approved,
                                               rdo._ExpMestSttId__Exported));

                        foreach (var gr in Groups)
                        {
                            var exp = rdo._ExpMests_Print.Where(o => gr.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                            if (exp != null && exp.Count > 0)
                            {
                                var grexp = exp.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                foreach (var item in grexp)
                                {
                                    var lst = gr.Where(o => item.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                                    ExpMestADO ado = new ExpMestADO(rdo.AggrExpMest, lst, rdo._ExpMestSttId__Approved, rdo._ExpMestSttId__Exported, item.First());
                                    ExpMestADOs.Add(ado);
                                }
                            }
                        }
                    }

                    if (dataMedis != null && dataMedis.Count > 0)
                    {
                        if (rdo._ConfigKeyOderOption == 1)
                        {
                            dataMedis = dataMedis.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                        }
                        else if (rdo._ConfigKeyOderOption == 2) 
                        {
                            dataMedis = dataMedis.OrderBy(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                        }
                        else if (rdo._ConfigKeyOderOption == 3)
                        {
                            dataMedis = dataMedis.OrderByDescending(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                        }
                        rdo.listAdo.AddRange(dataMedis);
                        SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.TOTAL_PRICE, dataMedis.Sum(o=>o.PRICE ?? 0)));
                        SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.TOTAL_PRICE_VAT, dataMedis.Sum(o => (o.PRICE ?? 0) * (1 + (o.VAT_RATIO ?? 0)))));
                    }
                }

                if (rdo._ExpMestMaterials != null && rdo._ExpMestMaterials.Count > 0)
                {
                    var query = rdo._ExpMestMaterials.ToList();
                    if (rdo.RoomIds != null && rdo.RoomIds.Count > 0 && rdo._ExpMests_Print != null && rdo._ExpMests_Print.Count > 0)
                    {
                        query = query.Where(p => rdo.RoomIds.Contains(p.REQ_ROOM_ID)).ToList();
                    }
                    query = query.Where(p => Check(p)).ToList();

                    List<Mps000046ADO> dataMates = new List<Mps000046ADO>();
                    if (rdo._ConfigKeyMERGER_DATA == 1)
                    {
                        var Groups = query.GroupBy(g => new
                    {
                        g.MATERIAL_ID
                    }).Select(p => p.ToList()).ToList();
                        dataMates.AddRange(from r in Groups
                                           select new Mps000046ADO(rdo.AggrExpMest,
                                               r,
                                               rdo._ExpMestSttId__Approved,
                                               rdo._ExpMestSttId__Exported));

                        foreach (var gr in Groups)
                        {
                            var exp = rdo._ExpMests_Print.Where(o => gr.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                            if (exp != null && exp.Count > 0)
                            {
                                var grexp = exp.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                foreach (var item in grexp)
                                {
                                    var lst = gr.Where(o => item.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                                    ExpMestADO ado = new ExpMestADO(rdo.AggrExpMest, lst, rdo._ExpMestSttId__Approved, rdo._ExpMestSttId__Exported, item.First());
                                    ExpMestADOs.Add(ado);
                                }
                            }
                        }
                    }
                    else
                    {
                        var Groups = query.GroupBy(g => new
                        {
                            g.MATERIAL_TYPE_ID
                        }).Select(p => p.ToList()).ToList();
                        dataMates.AddRange(from r in Groups
                                           select new Mps000046ADO(rdo.AggrExpMest,
                                               r,
                                               rdo._ExpMestSttId__Approved,
                                               rdo._ExpMestSttId__Exported));

                        foreach (var gr in Groups)
                        {
                            var exp = rdo._ExpMests_Print.Where(o => gr.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                            if (exp != null && exp.Count > 0)
                            {
                                var grexp = exp.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                foreach (var item in grexp)
                                {
                                    var lst = gr.Where(o => item.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                                    ExpMestADO ado = new ExpMestADO(rdo.AggrExpMest, lst, rdo._ExpMestSttId__Approved, rdo._ExpMestSttId__Exported, item.First());
                                    ExpMestADOs.Add(ado);
                                }
                            }
                        }
                    }

                    if (dataMates != null && dataMates.Count > 0)
                    {
                        dataMates = dataMates.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                        rdo.listAdo.AddRange(dataMates);
                        SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.TOTAL_PRICE, dataMates.Sum(o => o.PRICE ?? 0)));
                        SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.TOTAL_PRICE_VAT, dataMates.Sum(o => (o.PRICE ?? 0) * (1 + (o.VAT_RATIO ?? 0)))));
                    }
                }
                //if (rdo.listAdo != null && rdo.listAdo.Count > 0)
                //{
                //    rdo.listAdo = rdo.listAdo.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessListADO_LOGIN()
        {
            try
            {
                this.ReqLoginADOs = new List<ReqLoginADO>();
                this.listAdo_Logins = new List<Mps000046ADO>();
                this.ExpMestADO_Logins = new List<ExpMestADO>();
                if (rdo.IsMedicine && rdo._ExpMestMedicines != null && rdo._ExpMestMedicines.Count > 0)
                {
                    var query = rdo._ExpMestMedicines.ToList();
                    if (rdo.RoomIds != null && rdo.RoomIds.Count > 0)
                    {
                        query = query.Where(p => rdo.RoomIds.Contains(p.REQ_ROOM_ID)).ToList();
                    }
                    query = query.Where(p => Check(p)).ToList();

                    List<Mps000046ADO> dataMedis = new List<Mps000046ADO>();
                    if (rdo._ConfigKeyMERGER_DATA == 1)
                    {
                        var Groups = query.GroupBy(g => new
                        {
                            g.MEDICINE_ID,
                            g.CONCENTRA,
                            g.REQ_LOGINNAME
                        }).Select(p => p.ToList()).ToList();
                        dataMedis.AddRange(from r in Groups
                                           select new Mps000046ADO(rdo.AggrExpMest,
                                               r,
                                               rdo._ExpMestSttId__Approved,
                                               rdo._ExpMestSttId__Exported));

                        foreach (var gr in Groups)
                        {
                            var exp = rdo._ExpMests_Print.Where(o => gr.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                            if (exp != null && exp.Count > 0)
                            {
                                var grexp = exp.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                foreach (var item in grexp)
                                {
                                    var lst = gr.Where(o => item.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                                    ExpMestADO ado = new ExpMestADO(rdo.AggrExpMest, lst, rdo._ExpMestSttId__Approved, rdo._ExpMestSttId__Exported, item.First());
                                    ExpMestADO_Logins.Add(ado);
                                }
                            }
                        }
                    }
                    else
                    {
                        var Groups = query.GroupBy(g => new
                        {
                            g.MEDICINE_TYPE_ID,
                            g.CONCENTRA,
                            g.REQ_LOGINNAME
                        }).Select(p => p.ToList()).ToList();
                        dataMedis.AddRange(from r in Groups
                                           select new Mps000046ADO(rdo.AggrExpMest,
                                               r,
                                               rdo._ExpMestSttId__Approved,
                                               rdo._ExpMestSttId__Exported));

                        foreach (var gr in Groups)
                        {
                            var exp = rdo._ExpMests_Print.Where(o => gr.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                            if (exp != null && exp.Count > 0)
                            {
                                var grexp = exp.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                foreach (var item in grexp)
                                {
                                    var lst = gr.Where(o => item.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                                    ExpMestADO ado = new ExpMestADO(rdo.AggrExpMest, lst, rdo._ExpMestSttId__Approved, rdo._ExpMestSttId__Exported, item.First());
                                    ExpMestADO_Logins.Add(ado);
                                }
                            }
                        }
                    }

                    if (dataMedis != null && dataMedis.Count > 0)
                    {
                        if (rdo._ConfigKeyOderOption == 1)
                        {
                            dataMedis = dataMedis.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                        }
                        else if (rdo._ConfigKeyOderOption == 2)
                        {
                            dataMedis = dataMedis.OrderBy(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                        }
                        else if (rdo._ConfigKeyOderOption == 3)
                        {
                            dataMedis = dataMedis.OrderByDescending(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                        }
                        listAdo_Logins.AddRange(dataMedis);
                        SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.TOTAL_PRICE_LOGIN, dataMedis.Sum(o => o.PRICE ?? 0)));
                        SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.TOTAL_PRICE_VAT_LOGIN, dataMedis.Sum(o => (o.PRICE ?? 0) * (1 + (o.VAT_RATIO ?? 0)))));
                    }
                }

                if (rdo._ExpMestMaterials != null && rdo._ExpMestMaterials.Count > 0)
                {
                    var query = rdo._ExpMestMaterials.ToList();
                    if (rdo.RoomIds != null && rdo.RoomIds.Count > 0 && rdo._ExpMests_Print != null && rdo._ExpMests_Print.Count > 0)
                    {
                        query = query.Where(p => rdo.RoomIds.Contains(p.REQ_ROOM_ID)).ToList();
                    }
                    query = query.Where(p => Check(p)).ToList();

                    List<Mps000046ADO> dataMates = new List<Mps000046ADO>();
                    if (rdo._ConfigKeyMERGER_DATA == 1)
                    {
                        var Groups = query.GroupBy(g => new
                        {
                            g.MATERIAL_ID,
                            g.REQ_LOGINNAME
                        }).Select(p => p.ToList()).ToList();
                        dataMates.AddRange(from r in Groups
                                           select new Mps000046ADO(rdo.AggrExpMest,
                                               r,
                                               rdo._ExpMestSttId__Approved,
                                               rdo._ExpMestSttId__Exported));

                        foreach (var gr in Groups)
                        {
                            var exp = rdo._ExpMests_Print.Where(o => gr.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                            if (exp != null && exp.Count > 0)
                            {
                                var grexp = exp.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                foreach (var item in grexp)
                                {
                                    var lst = gr.Where(o => item.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                                    ExpMestADO ado = new ExpMestADO(rdo.AggrExpMest, lst, rdo._ExpMestSttId__Approved, rdo._ExpMestSttId__Exported, item.First());
                                    ExpMestADO_Logins.Add(ado);
                                }
                            }
                        }
                    }
                    else
                    {
                        var Groups = query.GroupBy(g => new
                        {
                            g.MATERIAL_TYPE_ID,
                            g.REQ_LOGINNAME
                        }).Select(p => p.ToList()).ToList();
                        dataMates.AddRange(from r in Groups
                                           select new Mps000046ADO(rdo.AggrExpMest,
                                               r,
                                               rdo._ExpMestSttId__Approved,
                                               rdo._ExpMestSttId__Exported));

                        foreach (var gr in Groups)
                        {
                            var exp = rdo._ExpMests_Print.Where(o => gr.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                            if (exp != null && exp.Count > 0)
                            {
                                var grexp = exp.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                foreach (var item in grexp)
                                {
                                    var lst = gr.Where(o => item.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                                    ExpMestADO ado = new ExpMestADO(rdo.AggrExpMest, lst, rdo._ExpMestSttId__Approved, rdo._ExpMestSttId__Exported, item.First());
                                    ExpMestADO_Logins.Add(ado);
                                }
                            }
                        }
                    }

                    if (dataMates != null && dataMates.Count > 0)
                    {
                        dataMates = dataMates.OrderBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                        listAdo_Logins.AddRange(dataMates);
                        SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.TOTAL_PRICE_LOGIN, dataMates.Sum(o => o.PRICE ?? 0)));
                        SetSingleKey(new KeyValue(Mps000046ExtendSingleKey.TOTAL_PRICE_VAT_LOGIN, dataMates.Sum(o => (o.PRICE ?? 0) * (1 + (o.VAT_RATIO ?? 0)))));
                    }
                }

                if (this.listAdo_Logins != null && this.listAdo_Logins.Count > 0)
                {
                    foreach (var item in this.listAdo_Logins)
                    {
                        ReqLoginADO ado = new ReqLoginADO(item, this.listAdo_Logins);

                        var check = this.ReqLoginADOs.Where(o => o.REQ_LOGINNAME == ado.REQ_LOGINNAME && o.TYPE_ID == ado.TYPE_ID).ToList();

                        if (check == null || check.Count <= 0)
                        {
                            this.ReqLoginADOs.Add(ado);
                        }
                    }

                }

                if (ExpMestADO_Logins != null && ExpMestADO_Logins.Count > 0)
                {
                    ExpMestADO_Logins = ExpMestADO_Logins.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool Check(V_HIS_EXP_MEST_MEDICINE _expMestMedicine)
        {
            bool result = false;
            try
            {
                var data = rdo._MedicineTypes.FirstOrDefault(p => p.ID == _expMestMedicine.MEDICINE_TYPE_ID);
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

        bool Check(V_HIS_EXP_MEST_MATERIAL _expMestMaterial)
        {
            bool result = false;
            try
            {
                if (_expMestMaterial != null)
                {
                    if (rdo.ServiceUnitIds != null && rdo.ServiceUnitIds.Count > 0 && (rdo.Ismaterial || rdo.IsChemicalSustance))
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
    }
}
