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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using Inventec.Common.QRCoder;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000421.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MPS.Processor.Mps000421
{
    public partial class Mps000421Processor : AbstractProcessor
    {
        Mps000421PDO rdo;
        List<V_HIS_EXP_BLTY_SERVICE> ExpBltyService1 = new List<V_HIS_EXP_BLTY_SERVICE>();
        List<V_HIS_EXP_BLTY_SERVICE> ExpBltyService2 = new List<V_HIS_EXP_BLTY_SERVICE>();
        public Mps000421Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000421PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
            {

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
        /// <returns></returns
        public override bool ProcessData()
        {
            bool result = false;
            
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                SetSingleKey();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                store.SetCommonFunctions();

                if (rdo.ExpMestBlood == null)
                {
                    rdo.ExpMestBlood = new List<V_HIS_EXP_MEST_BLOOD>();
                }
                if (rdo.ExpBltyService == null)
                {
                    rdo.ExpBltyService = new List<V_HIS_EXP_BLTY_SERVICE>();
                }
                else 
                {
                    for (int i = 0; i < rdo.ExpBltyService.Count; i++)
                    {
                        if (rdo.ExpBltyService.Count % 2 == 0)
                        {
                            if (i < rdo.ExpBltyService.Count / 2)
                            {
                                ExpBltyService1.Add(rdo.ExpBltyService[i]);
                            }
                            else
                            {
                                ExpBltyService2.Add(rdo.ExpBltyService[i]);
                            }
                        }
                        else
                        {

                            if (i <= rdo.ExpBltyService.Count / 2)
                            {
                                ExpBltyService1.Add(rdo.ExpBltyService[i]);
                            }
                            else
                            {
                                ExpBltyService2.Add(rdo.ExpBltyService[i]);
                            }
                        }
                    }
                    if (ExpBltyService1.Count >ExpBltyService2.Count)
                    {
                        ExpBltyService2.Add(new V_HIS_EXP_BLTY_SERVICE());
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("Dữ liệu ExpBltyService1: "+ Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ExpBltyService1), ExpBltyService1));
                Inventec.Common.Logging.LogSystem.Debug("Dữ liệu ExpBltyService1: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ExpBltyService2), ExpBltyService2));
                List<ExpMestBloodADO> lstADO = new List<ExpMestBloodADO>();
                foreach (var item in rdo.ExpMestBlood)
                {
                    ExpMestBloodADO ado = new ExpMestBloodADO(item);
                    lstADO.Add(ado);
                }
                objectTag.AddObjectData(store, "ExpMestBlood", lstADO);
                objectTag.AddObjectData(store, "ExpBltyService1", ExpBltyService1);
                objectTag.AddObjectData(store, "ExpBltyService2", ExpBltyService2);               

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
                var numberBloodTransf = 1;
                if (rdo.ListExpMest != null && rdo.ListExpMest.Count > 0)
                {
                    var empL = rdo.ListExpMest.Where(o => (o.FINISH_TIME ?? 0) <= (rdo.ExpMest.FINISH_TIME ?? 0) && o.EXP_MEST_STT_ID == 5 && o.TDL_TREATMENT_ID == rdo.ExpMest.TDL_TREATMENT_ID).ToList();
                    numberBloodTransf = empL.Count == 0 ? 1 : empL.Count;
                }
                SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.NUMBER_BLOOD_TRANSFUSIONS, numberBloodTransf)));
                if (rdo.Treatment != null)
                {
                    var PATIENT_TYPE = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(md => md.ID == rdo.Treatment.TDL_PATIENT_TYPE_ID);

                    SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.Patient.DOB))));
                    SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.D_O_B, rdo.Treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4))));
                    SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.GENDER_NAME, rdo.Treatment.TDL_PATIENT_GENDER_NAME)));
                    SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.ICD_NAME, rdo.Treatment.ICD_NAME)));
                    SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.TREATMENT_CODE, rdo.Treatment.TREATMENT_CODE)));
                    SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.TDL_PATIENT_NAME, rdo.Treatment.TDL_PATIENT_NAME)));
                    SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.TDL_PATIENT_ADDRESS, rdo.Treatment.TDL_PATIENT_ADDRESS)));
                    SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.PATIENT_TYPE, PATIENT_TYPE.PATIENT_TYPE_NAME)));

                    AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.Treatment);
                }

                if (rdo.Patient != null)
                {
                    SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.BLOOD_ABO_CODE, rdo.Patient.BLOOD_ABO_CODE)));
                    SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.BLOOD_HR_CODE, rdo.Patient.BLOOD_RH_CODE)));
                    AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.Patient);
                }

                if (rdo.ExpMest != null)
                {
                    //thông tin khoa phòng
                    var DEPARTMENT = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(md => md.ID == rdo.ExpMest.REQ_DEPARTMENT_ID);
                    //thông tin buồng giường
                    var ROOM = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(md => md.ID == rdo.ExpMest.REQ_ROOM_ID);

                    SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.DEPARTMENT_NAME, DEPARTMENT.DEPARTMENT_NAME)));
                    SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.ROOM_NAME, ROOM.ROOM_NAME)));
                    SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.EXP_MEST_CODE, rdo.ExpMest.EXP_MEST_CODE)));
                    Inventec.Common.BarcodeLib.Barcode bc = new Inventec.Common.BarcodeLib.Barcode(rdo.ExpMest.EXP_MEST_CODE);
                    bc.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    bc.IncludeLabel = false;
                    bc.Width = 120;
                    bc.Height = 40;
                    bc.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    bc.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    bc.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    bc.AlternateLabel = " ";
                    bc.IncludeLabel = true;

                    dicImage.Add(Mps000421ExtendSingleKey.BARCODE_EXP_MEST_CODE, bc);
                    AddObjectKeyIntoListkey<V_HIS_EXP_MEST>(rdo.ExpMest);
                }
          
                SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.Hours, DateTime.Now.Hour)));
                SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.Minute, DateTime.Now.Minute)));
                SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.Day, DateTime.Now.Day)));
                SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.Month, DateTime.Now.Month)));
                SetSingleKey((new KeyValue(Mps000421ExtendSingleKey.Year, DateTime.Now.Year)));

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
                if (rdo == null || rdo.Treatment == null)
                    return log;
                log = "Mã điều trị: " + rdo.Treatment.TREATMENT_CODE;
                if (rdo.ExpMest != null)
                {
                    log += " , Mã phiếu xuất: " + rdo.ExpMest.EXP_MEST_CODE;
                }
                if (rdo.ExpMestBlood != null)
                {
                    string bloodCodeStr = String.Join(",", rdo.ExpMestBlood.OrderBy(o => o.BLOOD_CODE).Where(o => !String.IsNullOrWhiteSpace(o.BLOOD_CODE)).Select(o => o.BLOOD_CODE));
                    log += " , Mã túi máu: " + bloodCodeStr;
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
                if (rdo != null && rdo.Treatment != null)
                {
                    // Mã biểu in
                    string printCode = "Mps000421";

                    // Mã điều trị
                    string treatmentCode = "TREATMENT_CODE:" + (rdo.Treatment.TREATMENT_CODE ?? "");

                    // Mã phiếu xuất máu
                    string expMestCode = "EXP_MEST_CODE:" + (rdo.ExpMest?.EXP_MEST_CODE ?? "");

                    // Danh sách mã túi máu (BLOOD_CODE)
                    List<string> bloodCodes = new List<string>();
                    if (rdo.ExpMestBlood != null && rdo.ExpMestBlood.Count > 0)
                    {
                        bloodCodes = rdo.ExpMestBlood
                            .Select(b => "BLOOD_CODE:" + b.BLOOD_CODE).ToList();
                    }

                    // Danh sách tổng hợp truyền máu (V_HIS_TRANSFUSION_SUM)
                    List<string> transfusionSums = new List<string>();
                    if (rdo.TransFusionSum != null && rdo.TransFusionSum.Count > 0)
                    {
                        transfusionSums = rdo.TransFusionSum
                            .OrderBy(t => t.ID)
                            .Select(t => "HIS_TRANSFUSION_SUM:" + t.ID)
                            .ToList();
                    }

                    // Danh sách truyền máu HIS_TRANSFUSION, sắp xếp theo ID tăng dần
                    List<string> transfusionDetails = new List<string>();
                    if (rdo.TransFusions != null && rdo.TransFusions.Count > 0)
                    {
                        transfusionDetails = rdo.TransFusions
                            .OrderBy(t => t.ID)
                            .Select(t => "HIS_TRANSFUSION:" + t.ID + ",")
                            .ToList();
                    }

                    // Ghép tất cả các phần tử thành chuỗi kết quả
                    List<string> parts = new List<string> { printCode, treatmentCode, expMestCode };
                    parts.AddRange(bloodCodes);
                    parts.AddRange(transfusionDetails);
                    parts.RemoveAll(string.IsNullOrWhiteSpace); // Loại bỏ phần tử rỗng

                    result = string.Join(" ", parts);
                }
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
            long typeId = 0;
            long mediMateTypeId = 0;

            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
                bool result = false;
                try
                {
                    long servicetypeId = Convert.ToInt64(parameters[0]);
                    long mediMateId = Convert.ToInt64(parameters[1]);

                    if (servicetypeId > 0 && mediMateId > 0)
                    {
                        if (this.typeId == servicetypeId && this.mediMateTypeId == mediMateId)
                        {
                            return true;
                        }
                        else
                        {
                            this.typeId = servicetypeId;
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
