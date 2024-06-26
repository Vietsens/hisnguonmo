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
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000269.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000269
{
    public class Mps000269Processor : AbstractProcessor
    {
        Mps000269PDO rdo;
        public Mps000269Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000269PDO)rdoBase;
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

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                store.SetCommonFunctions();

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
                AddObjectKeyIntoListkey(rdo.TreatmentView, false);

                if (rdo.PatientTypeAlter != null)
                    AddObjectKeyIntoListkey(rdo.PatientTypeAlter, false);
                if (rdo.ado != null)
                    AddObjectKeyIntoListkey(rdo.ado, false);
                if (rdo._HisSereServ != null)
                    AddObjectKeyIntoListkey(rdo._HisSereServ, false);
                long? day = 0;
                if (rdo.TreatmentView != null && rdo.TreatmentView.SICK_LEAVE_FROM > 0 && rdo.TreatmentView.SICK_LEAVE_TO > 0)
                {
                    day = DayOfTreatment(rdo.TreatmentView.SICK_LEAVE_FROM, rdo.TreatmentView.SICK_LEAVE_TO);
                    if (day != null)
                    {
                        day = day + 1;
                    }
                }
                SetSingleKey(new KeyValue(Mps000269ExtendSingleKey.SICK_NUM_DAY, day));
                AddObjectKeyIntoListkey(rdo.Patient, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long? DayOfTreatment(long? timeIn, long? timeOut)
        {
            long? result = null;
            try
            {
                if (!timeIn.HasValue || !timeOut.HasValue || timeIn > timeOut)
                    return result;

                DateTime dtIn = TimeNumberToSystemDateTime(timeIn.Value) ?? DateTime.Now;
                DateTime dtOut = TimeNumberToSystemDateTime(timeOut.Value) ?? DateTime.Now;
                TimeSpan ts = new TimeSpan();
                ts = (TimeSpan)(dtOut - dtIn);

                result = ts.Days;
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        private static System.DateTime? TimeNumberToSystemDateTime(long time)
        {
            System.DateTime? result = null;
            try
            {
                if (time > 0)
                {
                    result = System.DateTime.ParseExact(time.ToString(), "yyyyMMddHHmmss",
                                       System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }
    }
}
