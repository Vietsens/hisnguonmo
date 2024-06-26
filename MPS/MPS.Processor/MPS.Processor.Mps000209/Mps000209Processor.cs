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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000209.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000209
{
    class Mps000209Processor : AbstractProcessor
    {
        Mps000209PDO rdo;
        List<RoomCounterADO> roomCounterAdos = new List<RoomCounterADO>();

        public Mps000209Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000209PDO)rdoBase;
        }
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                ProcessRoomCounter();
                objectTag.AddObjectData(store, "ListRoomCounter", this.roomCounterAdos);
                objectTag.AddObjectData(store, "ListBedRoomCounter", rdo._bedRoomCounterSdos);
                objectTag.AddObjectData(store, "ListDepartment", rdo._departments);  

                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessRoomCounter()
        {
            try
            {
                foreach (var item in rdo._roomCounters)
                {
                    RoomCounterADO ado = new RoomCounterADO(item);
                    roomCounterAdos.Add(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessSingleKey()
        {
            try
            {
                if (this.rdo._mps000209Ado != null)
                {
                    SetSingleKey(new KeyValue(Mps000209ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.rdo._mps000209Ado.CREATE_TIME ?? 0)));

                    AddObjectKeyIntoListkey(this.rdo._mps000209Ado, false);
                    //SetSingleKey(new KeyValue(Mps000209ExtendSingleKey.DEPARTMENT_NAME_PRINT_STR, this.rdo._mps000209Ado.DEPARTMENT_NAME_PRINT));
                    //SetSingleKey(new KeyValue(Mps000209ExtendSingleKey.LIST_DEPARTMENT_NAME_STR, this.rdo._mps000209Ado.LIST_DEPARTMENT_NAME));
                    //SetSingleKey(new KeyValue(Mps000209ExtendSingleKey.LOGIN_NAME_PRINT_STR, this.rdo._mps000209Ado.LOGIN_NAME_PRINT));
                    //SetSingleKey(new KeyValue(Mps000209ExtendSingleKey.USERNAME_PRINT_STR, this.rdo._mps000209Ado.USERNAME_PRINT));
                    //SetSingleKey(new KeyValue(Mps000209ExtendSingleKey.ROOM_NAME_PRINT_STR, this.rdo._mps000209Ado.ROOM_NAME_PRINT));
                }
              
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
