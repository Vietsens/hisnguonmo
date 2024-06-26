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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000079.PDO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;

namespace MPS.Processor.Mps000079
{
  class Mps000079Processor : AbstractProcessor
  {
    Mps000079PDO rdo;

    internal Mps000079Processor(CommonParam param, PrintData printData)
      : base(param, printData)
    {
      rdo = (Mps000079PDO)rdoBase;
    }

    public void SetBarcodeKey()
    {
      try
      {
        Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentHisTreatment.TREATMENT_CODE);
        barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
        barcodeTreatmentCode.IncludeLabel = false;
        barcodeTreatmentCode.Width = 120;
        barcodeTreatmentCode.Height = 40;
        barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
        barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
        barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
        barcodeTreatmentCode.IncludeLabel = true;

        dicImage.Add(Mps000079ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatmentCode);

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

        store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
        SetSingleKey();
        singleTag.ProcessData(store, singleValueDictionary);
        barCodeTag.ProcessData(store, dicImage);
        objectTag.AddObjectData(store, "SereServHighTechs", rdo.sereServKTCADOs);
        objectTag.AddObjectData(store, "Services", rdo.sereServs);
        objectTag.AddObjectData(store, "ServiceGroups", rdo.sereServServices);
        objectTag.AddObjectData(store, "KTCFeeGroups", rdo.sereServKTCFees);
        objectTag.AddObjectData(store, "ExecuteGroups", rdo.sereServExecutes);

        objectTag.AddRelationship(store, "KTCFeeGroups", "ExecuteGroups", "IS_OUT_PARENT_FEE", "IS_OUT_PARENT_FEE");
        objectTag.AddRelationship(store, "ExecuteGroups", "ServiceGroups", "EXECUTE_GROUP_ID", "EXECUTE_GROUP_ID");
        objectTag.AddRelationship(store, "KTCFeeGroups", "ServiceGroups", "IS_OUT_PARENT_FEE", "IS_OUT_PARENT_FEE");
        objectTag.AddRelationship(store, "ServiceGroups", "Services", "HEIN_SERVICE_TYPE_ID", "HEIN_SERVICE_TYPE_ID");
        objectTag.AddRelationship(store, "ExecuteGroups", "Services", "EXECUTE_GROUP_ID", "EXECUTE_GROUP_ID");
        objectTag.AddRelationship(store, "KTCFeeGroups", "Services", "IS_OUT_PARENT_FEE", "IS_OUT_PARENT_FEE");

        result = true;
      }
      catch (Exception ex)
      {
        result = false;
        Inventec.Common.Logging.LogSystem.Error(ex);
      }

      return result;
    }

    class CustomerFuncRownumberData : TFlexCelUserFunction
    {
      public CustomerFuncRownumberData()
      {
      }
      public override object Evaluate(object[] parameters)
      {
        if (parameters == null || parameters.Length < 1)
          throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

        long result = 0;
        try
        {
          long rownumber = Convert.ToInt64(parameters[0]);
          result = (rownumber + 1);
        }
        catch (Exception ex)
        {
          LogSystem.Debug(ex);
        }

        return result;
      }
    }

    void SetSingleKey()
    {
      try
      {
        AddObjectKeyIntoListkey<PatientADO>(rdo.patientADO);
        if (rdo.departmentTrans != null && rdo.departmentTrans.Count > 0)
        {
          SetSingleKey(new KeyValue(Mps000079ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[0].LOG_TIME)));
          if (rdo.departmentTrans[rdo.departmentTrans.Count - 1] != null && rdo.departmentTrans.Count > 1)
          {
            SetSingleKey(new KeyValue(Mps000079ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[rdo.departmentTrans.Count - 1].LOG_TIME)));
            SetSingleKey(new KeyValue(Mps000079ExtendSingleKey.TOTAL_DAY, rdo.totalDay));

          }

          //Thời gian vào khoa
          List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTranTemps = new List<V_HIS_DEPARTMENT_TRAN>();
          foreach (var departmentTran in rdo.departmentTrans)
          {
            if (departmentTran != null)
              departmentTranTemps.Add(departmentTran);

          }

          var departmentIns = departmentTranTemps.Where(o => o.IN_OUT == 1).OrderByDescending(o => o.LOG_TIME).ToList();
          if (departmentIns != null && departmentIns.Count > 0)
          {
            var timeDepartmentIn = departmentIns[0].LOG_TIME;
            SetSingleKey(new KeyValue(Mps000079ExtendSingleKey.TIME_DEPARTMENT_IN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(timeDepartmentIn)));
          }

          if (departmentTranTemps.Count == rdo.departmentTrans.Count)
          {
            var departmentOuts = rdo.departmentTrans.Where(o => o.IN_OUT == 2).OrderByDescending(o => o.LOG_TIME).ToList();
            if (departmentOuts != null && departmentOuts.Count > 0)
            {
              var timeDepartmentOut = departmentOuts[0].LOG_TIME;

              SetSingleKey(new KeyValue(Mps000079ExtendSingleKey.TIME_DEPARTMENT_OUT, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(timeDepartmentOut)));
            }
          }
        }

        if (rdo.departmentName != null)
        {
          SetSingleKey(new KeyValue(Mps000079ExtendSingleKey.DEPARTMENT_NAME, rdo.departmentName));
        }



        if (rdo.patyAlterBhytADO != null)
        {
          if (rdo.patyAlterBhytADO.IS_HEIN != null)
            SetSingleKey(new KeyValue(Mps000079ExtendSingleKey.IS_HEIN, "X"));
          else
            SetSingleKey(new KeyValue(Mps000079ExtendSingleKey.IS_NOT_HEIN, "X"));
        }

        decimal tongTien = 0;
        decimal tongTienBHYT = 0;
        decimal tongTienBNChiTra = 0;
        foreach (var sereServ in rdo.sereServKTCFees)
        {
          tongTien += sereServ.TOTAL_PRICE_GROUP;
          tongTienBHYT += sereServ.TOTAL_HEIN_PRICE_SERVICE_GROUP;
          tongTienBNChiTra += sereServ.TOTAL_PATIENT_PRICE_SERVICE_GROUP;

        }
        SetSingleKey(new KeyValue(Mps000079ExtendSingleKey.TOTAL_PRICE, tongTien));
        SetSingleKey(new KeyValue(Mps000079ExtendSingleKey.TOTAL_PRICE_HEIN, tongTienBHYT));
        SetSingleKey(new KeyValue(Mps000079ExtendSingleKey.TOTAL_PRICE_PATIENT, tongTienBNChiTra));

        AddObjectKeyIntoListkey<PatyAlterBhytADO>(rdo.patyAlterBhytADO, false);
        AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.currentHisTreatment, false);
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    void ProcessGroupSereServ()
    {
      try
      {
        rdo.sereServKTCADOs = rdo.sereServKTCADOs.Where(o => o.IS_NO_EXECUTE == null).ToList();
        rdo.sereServs = rdo.sereServs.Where(o => o.IS_NO_EXECUTE == null).ToList();

        rdo.sereServKTCFees = new List<SereServGroupPlusADO>();
        rdo.sereServExecutes = new List<SereServGroupPlusADO>();
        rdo.sereServServices = new List<SereServGroupPlusADO>();

        //Nhom chi phi trong, ngoai goi theo dich vu KTC
        var sereServKTCGroups = rdo.sereServs.GroupBy(o => o.IS_OUT_PARENT_FEE).ToList();
        foreach (var sereServKTCGroup in sereServKTCGroups)
        {
          List<V_HIS_SERE_SERV> subSereServKTC = sereServKTCGroup.ToList<V_HIS_SERE_SERV>();

          SereServGroupPlusADO ktcFeeGroup = new SereServGroupPlusADO();
          AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV, SereServGroupPlusADO>();
          ktcFeeGroup = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV, SereServGroupPlusADO>(subSereServKTC.First());
          if (ktcFeeGroup.IS_OUT_PARENT_FEE == null)
          {
            ktcFeeGroup.KTC_FEE_GROUP_NAME = "CHI PHÍ TRONG GÓI PHẪU THUẬT";
          }
          else
          {
            ktcFeeGroup.KTC_FEE_GROUP_NAME = "CHI PHÍ NGOÀI GÓI PHẪU THUẬT";
          }
          //Tong tien theo Goi
          ktcFeeGroup.TOTAL_PRICE_GROUP = subSereServKTC.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND) ?? 0;
          ktcFeeGroup.TOTAL_HEIN_PRICE_SERVICE_GROUP = subSereServKTC.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
          ktcFeeGroup.TOTAL_PATIENT_PRICE_SERVICE_GROUP = subSereServKTC.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
          rdo.sereServKTCFees.Add(ktcFeeGroup);

          //Nhom ExecuteGroup
          var sereServExecuteGroups = subSereServKTC.GroupBy(o => o.EXECUTE_GROUP_ID).ToList();
          foreach (var sereServExecuteGroup in sereServExecuteGroups)
          {
            List<V_HIS_SERE_SERV> subSereServExecutes = sereServExecuteGroup.ToList<V_HIS_SERE_SERV>();
            SereServGroupPlusADO executeGroup = new SereServGroupPlusADO();
            AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV, SereServGroupPlusADO>();
            executeGroup = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV, SereServGroupPlusADO>(subSereServExecutes.First());
            if (executeGroup.EXECUTE_GROUP_ID == null)
            {
              executeGroup.EXECUTE_GROUP_NAME = "Khác";
            }
            else
            {
              executeGroup.EXECUTE_GROUP_NAME = rdo.executeGroups.FirstOrDefault(o => o.ID == executeGroup.EXECUTE_GROUP_ID).EXECUTE_GROUP_NAME;
            }
            rdo.sereServExecutes.Add(executeGroup);

            //Nhom ServiceGroup
            var sereServServiceGroups = subSereServExecutes.GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
            foreach (var sereServService in sereServServiceGroups)
            {
              List<V_HIS_SERE_SERV> subSereServServices = sereServService.ToList<V_HIS_SERE_SERV>();
              SereServGroupPlusADO serviceGroup = new SereServGroupPlusADO();
              AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV, SereServGroupPlusADO>();
              serviceGroup = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV, SereServGroupPlusADO>(subSereServServices.First());
              if (serviceGroup.HEIN_SERVICE_TYPE_ID == null)
                serviceGroup.HEIN_SERVICE_TYPE_NAME = "Khác";
              else
                serviceGroup.HEIN_SERVICE_TYPE_NAME = "Tiền " + rdo.ServiceReports.FirstOrDefault(o => o.ID == serviceGroup.HEIN_SERVICE_TYPE_ID).HEIN_SERVICE_TYPE_NAME;

              serviceGroup.TOTAL_PRICE_SERVICE_GROUP = subSereServServices.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND) ?? 0;
              serviceGroup.TOTAL_HEIN_PRICE_SERVICE_GROUP = subSereServServices.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
              serviceGroup.TOTAL_PATIENT_PRICE_SERVICE_GROUP = subSereServServices.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
              rdo.sereServServices.Add(serviceGroup);
            }

          }
        }

      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Warn(ex);
      }
    }
  }
}
