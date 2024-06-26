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
using MPS.Processor.Mps000066.PDO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000066
{
	class Mps000066Processor : AbstractProcessor
	{
		Mps000066PDO rdo;

		public Mps000066Processor(CommonParam param, PrintData printData)
			: base(param, printData)
		{
			rdo = (Mps000066PDO)rdoBase;
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
				//barCodeTag.ProcessData(store, dicImage);				
				objectTag.AddObjectData<HIS_DEBATE_USER>(store, "Users", rdo.lstHisDebateUserTGia);				
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
				List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> listPresidentSecretary = rdo.lstHisDebateUser.Where(o => o.IS_PRESIDENT == 1 || o.IS_SECRETARY == 1).ToList();
				if (listPresidentSecretary != null)
				{
					foreach (var item_User in listPresidentSecretary)
					{
						if (item_User.IS_PRESIDENT == 1)
						{
							SetSingleKey(new KeyValue(Mps000066ExtendSingleKey.USERNAME_PRESIDENT, item_User.USERNAME));
							SetSingleKey(new KeyValue(Mps000066ExtendSingleKey.PRESIDENT_DESCRIPTION, item_User.DESCRIPTION));
						}
						if (item_User.IS_SECRETARY == 1)
						{
							SetSingleKey(new KeyValue(Mps000066ExtendSingleKey.USERNAME_SECRETARY, item_User.USERNAME));
							SetSingleKey(new KeyValue(Mps000066ExtendSingleKey.SECRETARY_DESCRIPTION, item_User.DESCRIPTION));
						}
					}
				}
				if (rdo.HisDebateRow != null)
				{
					SetSingleKey(new KeyValue(Mps000066ExtendSingleKey.DEBATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.HisDebateRow.DEBATE_TIME ?? 0)));
				}
				else
				{
					SetSingleKey(new KeyValue(Mps000066ExtendSingleKey.DEBATE_TIME_STR, ""));
				}				
				AddObjectKeyIntoListkey<DepartmentTranADO>(rdo.departmentTran,false);
				SetSingleKey(new KeyValue(Mps000066ExtendSingleKey.BED_ROOM_NAME, rdo.bedRoomName));
				AddObjectKeyIntoListkey<HIS_DEBATE>(rdo.HisDebateRow,false);
				AddObjectKeyIntoListkey<PatyAlterBhytADO>(rdo.PatyAlterBhyt);
				AddObjectKeyIntoListkey<PatientADO>(rdo.Patient);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
	}
}
