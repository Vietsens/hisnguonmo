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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using MPS.Processor.Mps000138.ADO;
using MPS.Processor.Mps000138.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000138
{
    public class Mps000138Processor : AbstractProcessor
    {
        Mps000138PDO rdo;

        public Mps000138Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000138PDO)rdoBase;
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
                if (rdo._RegisterReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000138ExtendSingleKey.REGISTER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._RegisterReq.REGISTER_TIME)));
                    SetSingleKey(new KeyValue(Mps000138ExtendSingleKey.REGISTER_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._RegisterReq.REGISTER_TIME)));
                    AddObjectKeyIntoListkey<V_HIS_REGISTER_REQ>(rdo._RegisterReq, false);
                }
                SetSingleKey(new KeyValue(Mps000138ExtendSingleKey.LAST_CALLED_NUM_ORDER, rdo._RegisterReqLastCalled != null ? rdo._RegisterReqLastCalled.NUM_ORDER : 0));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
