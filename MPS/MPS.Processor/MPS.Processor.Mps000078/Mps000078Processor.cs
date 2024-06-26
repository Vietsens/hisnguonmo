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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MPS.Processor.Mps000078.PDO;
using MPS.Processor.Mps000078;
using MOS.EFMODEL.DataModels;

namespace MPS.Processor.Mps000078
{
    class Mps000078Processor : AbstractProcessor
    {
        Mps000078PDO rdo;
        List<Mps000078ADO> ImpMestManuMedicineSumForPrints = new List<Mps000078ADO>();
        List<ImpMestADO> ImpMestADOs = new List<ImpMestADO>();

        List<Mps000078ADO> ImpMestManuMedicineSumForPrint_Logins = new List<Mps000078ADO>();
        List<ImpMestADO> ImpMestADO_Logins = new List<ImpMestADO>();
        List<ReqLoginADO> ReqLoginADOs = new List<ReqLoginADO>();

        public Mps000078Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000078PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode impMestCodeBar = new Inventec.Common.BarcodeLib.Barcode(rdo.AggrImpMest.IMP_MEST_CODE);
                impMestCodeBar.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                impMestCodeBar.IncludeLabel = false;
                impMestCodeBar.Width = 120;
                impMestCodeBar.Height = 40;
                impMestCodeBar.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                impMestCodeBar.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                impMestCodeBar.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                impMestCodeBar.IncludeLabel = true;

                dicImage.Add(Mps000078ExtendSingleKey.IMP_MEST_CODE_BARCODE, impMestCodeBar);

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
                ProcessListADO_Login();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                SetSingleKey();
                SetBarcodeKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                if (ImpMestADOs != null && ImpMestADOs.Count > 0)
                {
                    ImpMestADOs = ImpMestADOs.OrderBy(o => o.TDL_PATIENT_FIRST_NAME).ToList();
                }

                if (ImpMestADO_Logins != null && ImpMestADO_Logins.Count > 0)
                {
                    ImpMestADO_Logins = ImpMestADO_Logins.OrderBy(o => o.TDL_PATIENT_FIRST_NAME).ToList();
                }

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ImpMestAggregates", ImpMestManuMedicineSumForPrints);
                objectTag.AddObjectData(store, "ImpMests", this.ImpMestADOs);

                objectTag.AddObjectData(store, "ReqLogin", this.ReqLoginADOs);
                objectTag.AddObjectData(store, "ImpMestAggLogin", this.ImpMestManuMedicineSumForPrint_Logins);
                objectTag.AddObjectData(store, "ImpMestLogin", this.ImpMestADO_Logins);

                objectTag.AddRelationship(store, "ImpMestAggregates", "ImpMests", new string[] { "MEDICINE_TYPE_ID", "IS_MEDICINE" }, new string[] { "MEDICINE_TYPE_ID", "IS_MEDICINE" });

                objectTag.AddRelationship(store, "ReqLogin", "ImpMestAggLogin", new string[] { "REQ_LOGINNAME", "IS_MEDICINE" }, new string[] { "REQ_LOGINNAME", "IS_MEDICINE" });
                objectTag.AddRelationship(store, "ReqLogin", "ImpMestLogin", new string[] { "REQ_LOGINNAME", "IS_MEDICINE" }, new string[] { "REQ_LOGINNAME", "IS_MEDICINE" });
                objectTag.AddRelationship(store, "ImpMestAggLogin", "ImpMestLogin", new string[] { "MEDICINE_TYPE_ID", "IS_MEDICINE" }, new string[] { "MEDICINE_TYPE_ID", "IS_MEDICINE" });

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
                ImpMestManuMedicineSumForPrints = new List<Mps000078ADO>();
                ImpMestADOs = new List<ImpMestADO>();
                if (rdo.IsMedicine && rdo._ImpMestMedicines != null && rdo._ImpMestMedicines.Count > 0)
                {
                    rdo._ImpMestMedicines = rdo._ImpMestMedicines.Where(p => Check(p)).ToList();

                    if (rdo.RoomIds != null && rdo.RoomIds.Count > 0)
                    {
                        rdo._ImpMestMedicines = rdo._ImpMestMedicines.Where(o => rdo.RoomIds.Contains(o.REQ_ROOM_ID ?? 0)).ToList();
                    }

                    var dataGroups = rdo._ImpMestMedicines.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                    ImpMestManuMedicineSumForPrints.AddRange((from r in dataGroups select new Mps000078ADO(r, rdo.AggrImpMest.IMP_MEST_STT_ID, rdo.HisImpMestSttId__Imported, rdo.HisImpMestSttId__Approved)).ToList());

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

                if (rdo.Ismaterial && rdo._ImpMestMaterials != null && rdo._ImpMestMaterials.Count > 0)
                {
                    if (rdo.RoomIds != null && rdo.RoomIds.Count > 0)
                    {
                        rdo._ImpMestMaterials = rdo._ImpMestMaterials.Where(o => rdo.RoomIds.Contains(o.REQ_ROOM_ID ?? 0)).ToList();
                    }

                    rdo._ImpMestMaterials = rdo._ImpMestMaterials.Where(p => Check(p)).ToList();

                    var dataGroups = rdo._ImpMestMaterials.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                    ImpMestManuMedicineSumForPrints.AddRange((from r in dataGroups select new Mps000078ADO(r, rdo.AggrImpMest.IMP_MEST_STT_ID, rdo.HisImpMestSttId__Imported, rdo.HisImpMestSttId__Approved)).ToList());

                    foreach (var item in rdo._ImpMestMaterials)
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

        void ProcessListADO_Login()
        {
            try
            {
                ImpMestManuMedicineSumForPrint_Logins = new List<Mps000078ADO>();
                ImpMestADO_Logins = new List<ImpMestADO>();
                this.ReqLoginADOs = new List<ReqLoginADO>();
                if (rdo.IsMedicine && rdo._ImpMestMedicines != null && rdo._ImpMestMedicines.Count > 0)
                {
                    rdo._ImpMestMedicines = rdo._ImpMestMedicines.Where(p => Check(p)).ToList();

                    if (rdo.RoomIds != null && rdo.RoomIds.Count > 0)
                    {
                        rdo._ImpMestMedicines = rdo._ImpMestMedicines.Where(o => rdo.RoomIds.Contains(o.REQ_ROOM_ID ?? 0)).ToList();
                    }

                    var dataGroups = rdo._ImpMestMedicines.GroupBy(p => new { p.MEDICINE_TYPE_ID, p.REQ_LOGINNAME }).Select(p => p.ToList()).ToList();
                    ImpMestManuMedicineSumForPrint_Logins.AddRange((from r in dataGroups select new Mps000078ADO(r, rdo.AggrImpMest.IMP_MEST_STT_ID, rdo.HisImpMestSttId__Imported, rdo.HisImpMestSttId__Approved)).ToList());

                    foreach (var item in rdo._ImpMestMedicines)
                    {
                        var imp = rdo._ImpMests_Print.FirstOrDefault(o => o.ID == item.IMP_MEST_ID);
                        if (imp != null)
                        {
                            ImpMestADO ado = new ImpMestADO(item, rdo.AggrImpMest.IMP_MEST_STT_ID, rdo.HisImpMestSttId__Imported, rdo.HisImpMestSttId__Approved, imp);
                            ImpMestADO_Logins.Add(ado);
                        }
                    }
                }

                if (rdo.Ismaterial && rdo._ImpMestMaterials != null && rdo._ImpMestMaterials.Count > 0)
                {
                    if (rdo.RoomIds != null && rdo.RoomIds.Count > 0)
                    {
                        rdo._ImpMestMaterials = rdo._ImpMestMaterials.Where(o => rdo.RoomIds.Contains(o.REQ_ROOM_ID ?? 0)).ToList();
                    }

                    rdo._ImpMestMaterials = rdo._ImpMestMaterials.Where(p => Check(p)).ToList();

                    var dataGroups = rdo._ImpMestMaterials.GroupBy(p => new { p.MATERIAL_TYPE_ID, p.REQ_LOGINNAME }).Select(p => p.ToList()).ToList();
                    ImpMestManuMedicineSumForPrint_Logins.AddRange((from r in dataGroups select new Mps000078ADO(r, rdo.AggrImpMest.IMP_MEST_STT_ID, rdo.HisImpMestSttId__Imported, rdo.HisImpMestSttId__Approved)).ToList());

                    foreach (var item in rdo._ImpMestMaterials)
                    {
                        var imp = rdo._ImpMests_Print.FirstOrDefault(o => o.ID == item.IMP_MEST_ID);
                        if (imp != null)
                        {
                            ImpMestADO ado = new ImpMestADO(item, rdo.AggrImpMest.IMP_MEST_STT_ID, rdo.HisImpMestSttId__Imported, rdo.HisImpMestSttId__Approved, imp);
                            ImpMestADO_Logins.Add(ado);
                        }
                    }
                }

                if (ImpMestManuMedicineSumForPrint_Logins != null && ImpMestManuMedicineSumForPrint_Logins.Count > 0)
                {
                    ImpMestManuMedicineSumForPrint_Logins = ImpMestManuMedicineSumForPrint_Logins.OrderBy(p => p.MEDICINE_TYPE_NAME).ToList();

                    foreach (var item in ImpMestManuMedicineSumForPrint_Logins)
                    {
                        ReqLoginADO ado = new ReqLoginADO(item, ImpMestManuMedicineSumForPrint_Logins);

                        var check = this.ReqLoginADOs.Where(o => o.REQ_LOGINNAME == ado.REQ_LOGINNAME && o.IS_MEDICINE == ado.IS_MEDICINE).ToList();

                        if (check == null || check.Count <= 0)
                        {
                            this.ReqLoginADOs.Add(ado);
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ReqLoginADOs), ReqLoginADOs));

                if (ImpMestADO_Logins != null && ImpMestADO_Logins.Count > 0)
                {
                    ImpMestADO_Logins = ImpMestADO_Logins.Distinct().ToList();
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
                var data = rdo._MedicineTypes.FirstOrDefault(p => p.ID == _medi.MEDICINE_TYPE_ID);
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

        bool Check(V_HIS_IMP_MEST_MATERIAL _mate)
        {
            bool result = false;
            try
            {
                if (_mate != null)
                {
                    if (rdo.ServiceUnitIds != null && rdo.ServiceUnitIds.Count > 0)
                    {
                        if (rdo.ServiceUnitIds.Contains(_mate.SERVICE_UNIT_ID))
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

        void SetSingleKey()
        {
            try
            {
                if (rdo._MobaExpMests != null && rdo._MobaExpMests.Count > 0)
                {
                    var minTime = rdo._MobaExpMests.Min(p => p.TDL_INTRUCTION_TIME ?? 0);
                    var maxTime = rdo._MobaExpMests.Max(p => p.TDL_INTRUCTION_TIME ?? 0);
                    SetSingleKey(new KeyValue(Mps000078ExtendSingleKey.MIN_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minTime)));
                    SetSingleKey(new KeyValue(Mps000078ExtendSingleKey.MIN_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(minTime)));
                    SetSingleKey(new KeyValue(Mps000078ExtendSingleKey.MAX_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000078ExtendSingleKey.MAX_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxTime)));
                }
                string name = "";
                switch (rdo.keyNameTitles)
                {
                    case IsTittle.TongHop:
                        name = "TỔNG HỢP";
                        break;
                    case IsTittle.ThuocThuong:
                        name = "THUỐC THƯỜNG";
                        break;
                    case IsTittle.VatTu:
                        name = "VẬT TƯ";
                        break;
                    case IsTittle.Corticoid:
                        name = "CORTICOID";
                        break;
                    case IsTittle.KhangSinh:
                        name = "KHÁNG SINH";
                        break;
                    case IsTittle.Lao:
                        name = "LAO";
                        break;
                    case IsTittle.DichTruyen:
                        name = "DỊCH TRUYỀN";
                        break;
                    case IsTittle.TienChat:
                        name = "TIỀN CHẤT";
                        break;
                    default:
                        break;
                }

                SetSingleKey(new KeyValue(Mps000078ExtendSingleKey.KEY_NAME_TITLES, name));
                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(rdo.AggrImpMest, false);
                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(rdo.Department, false);

                SetSingleKey(new KeyValue(Mps000078ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.AggrImpMest.CREATE_TIME ?? 0)));

                decimal totalPrice = 0;
                if (ImpMestManuMedicineSumForPrints != null && ImpMestManuMedicineSumForPrints.Count > 0)
                {
                    totalPrice = ImpMestManuMedicineSumForPrints.Sum(s => s.TOTAL_IMP_PRICE);
                }

                SetSingleKey(new KeyValue(Mps000078ExtendSingleKey.TOTAL_IMP_PRICE, totalPrice));
                SetSingleKey(new KeyValue(Mps000078ExtendSingleKey.TOTAL_IMP_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice)))));
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
                    log = LogDataImpMest("", rdo.AggrImpMest.IMP_MEST_CODE, printTypeCode);
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
                    case IsTittle.TongHop:
                        name = "TH";
                        break;
                    case IsTittle.ThuocThuong:
                        name = "TT";
                        break;
                    case IsTittle.VatTu:
                        name = "VT";
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
