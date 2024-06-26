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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000034.PDO;
using Inventec.Core;

namespace MPS.Processor.Mps000034
{
	class Mps000034Processor : AbstractProcessor
	{
		Mps000034PDO rdo;

		public Mps000034Processor(CommonParam param, PrintData printData)
			: base(param, printData)
		{
			rdo = (Mps000034PDO)rdoBase;
		}

		public void SetBarcodeKey()
		{
			try
			{
				Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode();
				if (rdo.ExamServiceReqResult != null)
				{
					Inventec.Common.BarcodeLib.Barcode PatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.ExamServiceReqResult.HisExamServiceReq.SERVICE_REQ_CODE);
					barcodePatientCode = PatientCode;
				}
				else
				{
					Inventec.Common.BarcodeLib.Barcode PatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.lstServiceReq.SERVICE_REQ_CODE);
					barcodePatientCode = PatientCode;
				}
				if (barcodePatientCode != null)
				{
					barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
					barcodePatientCode.IncludeLabel = false;
					barcodePatientCode.Width = 120;
					barcodePatientCode.Height = 40;
					barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
					barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
					barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
					barcodePatientCode.IncludeLabel = true;

					dicImage.Add(Mps000034ExtendSingleKey.PATIENT_CODE_BAR, barcodePatientCode);
				}

				//Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.currentHisTreatment.TREATMENT_CODE);
				//barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
				//barcodeTreatment.IncludeLabel = false;
				//barcodeTreatment.Width = 120;
				//barcodeTreatment.Height = 40;
				//barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
				//barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
				//barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
				//barcodeTreatment.IncludeLabel = true;

				//dicImage.Add(Mps000034ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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
				//objectTag.AddObjectData(store, "SereServ", rdo.sereServs);



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
				if (rdo.ExamServiceReqResult != null)
				{
					if (rdo.ExamServiceReqResult.HisExamServiceReq.INTRUCTION_TIME != null)
					{
						SetSingleKey(new KeyValue(Mps000034ExtendSingleKey.REQUEST_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((rdo.ExamServiceReqResult.HisExamServiceReq.INTRUCTION_TIME))));
					}
					if (rdo.ExamServiceReqResult.HisExamServiceReq.SERVICE_REQ_CODE != null)
					{
						SetSingleKey(new KeyValue(Mps000034ExtendSingleKey.SERVICE_REQ_CODE, rdo.ExamServiceReqResult.HisExamServiceReq.SERVICE_REQ_CODE));
					}
					if (rdo.ExamServiceReqResult.HisExamServiceReq.EXECUTE_ROOM_NAME != null)
					{
						SetSingleKey(new KeyValue(Mps000034ExtendSingleKey.EXECUTE_ROOM_NAME, rdo.ExamServiceReqResult.HisExamServiceReq.EXECUTE_ROOM_NAME));
					}
					if (rdo.ExamServiceReqResult.HisExamServiceReq.NUM_ORDER != null)
					{
						SetSingleKey(new KeyValue(Mps000034ExtendSingleKey.NUM_ORDER, rdo.ExamServiceReqResult.HisExamServiceReq.NUM_ORDER));
					}
				}

				AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(rdo.department, false);
				AddObjectKeyIntoListkey<PatientADO>(rdo.patientADO, false);
				AddObjectKeyIntoListkey<PatyAlterBhytADO>(rdo.patyAlterBhytADO, false);
				if (rdo.ExamServiceReqResult == null)
				{
					if (rdo.lstServiceReq.INTRUCTION_TIME != null)
					{
						SetSingleKey(new KeyValue(Mps000034ExtendSingleKey.REQUEST_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((rdo.lstServiceReq.INTRUCTION_TIME))));
					}
					AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>(rdo.lstServiceReq);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

	}
}
