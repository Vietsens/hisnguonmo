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
using MPS.Processor.Mps000072.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000072
{
  class Mps000072Processor : AbstractProcessor
  {
    Mps000072PDO rdo;

    public Mps000072Processor(CommonParam param, PrintData printData)
			: base(param, printData)
		{
			rdo = (Mps000072PDO)rdoBase;
		}

    public void SetBarcodeKey()
    {
      try
      {
        Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentPatient.PATIENT_CODE);
        barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
        barcodePatientCode.IncludeLabel = false;
        barcodePatientCode.Width = 120;
        barcodePatientCode.Height = 40;
        barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
        barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
        barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
        barcodePatientCode.IncludeLabel = true;

        dicImage.Add(Mps000072ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

        Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.TREATMENT_CODE);
        barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
        barcodeTreatment.IncludeLabel = false;
        barcodeTreatment.Width = 120;
        barcodeTreatment.Height = 40;
        barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
        barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
        barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
        barcodeTreatment.IncludeLabel = true;

        dicImage.Add(Mps000072ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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
        Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
        Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
        Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

        store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
        SetSingleKey();
        singleTag.ProcessData(store, singleValueDictionary);
        barCodeTag.ProcessData(store, dicImage);

        objectTag.AddObjectData(store, "TrackingADOs", rdo.mps000062ADOs);
        objectTag.AddObjectData(store, "Medicines", rdo.lstMps000062ADOMedicines);
        objectTag.AddObjectData(store, "ServiceCLS", rdo.lstMps000062ADOServiceCLS);
        objectTag.AddRelationship(store, "TrackingADOs", "Medicines", "ID", "TRACKING_ID");
        objectTag.AddRelationship(store, "TrackingADOs", "ServiceCLS", "ID", "TRACKING_ID");

        objectTag.SetUserFunction(store, "FuncSameTitleCol1", new CustomerFuncMergeSameData(rdo.lstMps000062ADOMedicines, rdo.lstMps000062ADOServiceCLS, 1));
        objectTag.SetUserFunction(store, "FuncSameTitleCol2", new CustomerFuncMergeSameData(rdo.lstMps000062ADOMedicines, rdo.lstMps000062ADOServiceCLS, 2));

        result = true;
      }
      catch (Exception ex)
      {
        result = false;
        Inventec.Common.Logging.LogSystem.Error(ex);
      }

      return result;
    }

    class CustomerFuncMergeSameData : TFlexCelUserFunction
    {
      List<Mps000062ADOMedicines> ADOMedicines;
      List<Mps000062ADOServiceCLS> ADOServiceCLS;
      int SameType;
      public CustomerFuncMergeSameData(List<Mps000062ADOMedicines> medicines, List<Mps000062ADOServiceCLS> serviceCLS, int sameType)
      {
        ADOMedicines = medicines;
        ADOServiceCLS = serviceCLS;
        SameType = sameType;
      }
      public override object Evaluate(object[] parameters)
      {
        if (parameters == null || parameters.Length <= 0)
          throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

        bool result = false;
        try
        {
          int rowIndex = (int)parameters[0];
          if (rowIndex > 0)
          {
            switch (SameType)
            {
              case 1:
                if (ADOMedicines[rowIndex].TRACKING_ID == ADOMedicines[rowIndex - 1].TRACKING_ID)
                {
                  result = true;
                }
                else
                {
                  result = false;
                }
                break;
              case 2:
                if (ADOMedicines[rowIndex].TRACKING_ID == ADOMedicines[rowIndex - 1].TRACKING_ID)
                {
                  result = true;
                }
                else
                {
                  result = false;
                }
                break;

              default:
                break;
            }
          }
        }
        catch (Exception ex)
        {

        }

        return result;
      }
    }

    void SetSingleKey()
    {
      try
      {
        SetSingleKey(new KeyValue(Mps000072ExtendSingleKey.DEPARTMENT_NAME, rdo.departmentName));
        SetSingleKey(new KeyValue(Mps000072ExtendSingleKey.ROOM_NAME, rdo.roomName));
        AddObjectKeyIntoListkey<TreatmentADO>(rdo.currentTreatment, false);
        AddObjectKeyIntoListkey<PatientADO>(rdo.currentPatient);
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }
  }
}
