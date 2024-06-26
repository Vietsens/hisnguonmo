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
using Inventec.Core;
using MPS.Processor.Mps000067.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000067
{
	class Mps000067Processor : AbstractProcessor
	{
		Mps000067PDO rdo;

		public Mps000067Processor(CommonParam param, PrintData printData)
			: base(param, printData)
		{
			rdo = (Mps000067PDO)rdoBase;
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

		public override bool ProcessData()
		{
			bool result = false;
			try
			{
				Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
				Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
				Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

				store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
				ProcessSingleKey();
				singleTag.ProcessData(store, singleValueDictionary);
				result = true;
			}
			catch (Exception ex)
			{
				result = false;
				Inventec.Common.Logging.LogSystem.Error(ex);
			}

			return result;
		}

		void ProcessSingleKey()
		{
			try
			{				
				if (rdo.expMestMedicines != null)
					AddObjectKeyIntoListkey<ExeExpMestMedicineSDO>(rdo.expMestMedicines, false);
				if (rdo.expMestMedicine != null)
					AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>(rdo.expMestMedicine,false);
				AddObjectKeyIntoListkey<PatientADO>(rdo.Patient);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
	}
}
