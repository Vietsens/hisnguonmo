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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.Processor.Mps000068.PDO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;

namespace MPS.Processor.Mps000068
{
	class Mps000068Processor : AbstractProcessor
	{
		Mps000068PDO rdo;

		public Mps000068Processor(CommonParam param, PrintData printData)
			: base(param, printData)
		{
			rdo = (Mps000068PDO)rdoBase;
		}

		public override bool ProcessData()
		{
			bool result = false;
			try
			{
				Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
				Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

				store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
				SetSingleKey();
				singleTag.ProcessData(store, singleValueDictionary);
				barCodeTag.ProcessData(store, dicImage);

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
				//keyValues.Add((new KeyValue(Mps000068ExtendSingleKey.CREATE_TIME_TRAN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ExamServiceReqs.INTRUCTION_TIME))));
				AddObjectKeyIntoListkey<V_HIS_EXAM_SERVICE_REQ>(rdo.examServiceReq, false);
				AddObjectKeyIntoListkey<V_HIS_SERE_SERV>(rdo.sereServ, false);
				AddObjectKeyIntoListkey<HIS_DHST>(rdo.dhst, false);
				AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(rdo.tranPati, false);
				AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(rdo.tranPatiRaVien, false);
				AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(rdo.tranPatiOut, false);
				AddObjectKeyIntoListkey<HIS_TRAN_PATI_REASON>(rdo.tranPatiReason, false);
				AddObjectKeyIntoListkey<HIS_TRAN_PATI_FORM>(rdo.tranPatiForm, false);
				AddObjectKeyIntoListkey<TreatmentADO>(rdo.treatmentADO, false);
				AddObjectKeyIntoListkey<PatientADO>(rdo.currentPatient);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
	}
}
