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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000482.PDO
{
    public partial class Mps000482PDO: RDOBase
    {
        public List<Mps000482RDO> _mrs000482RDOs { get; set; }
        public SingKey482 _SingKey482 { get; set; }
        public List<Mps000482RDO> _ListBloods { get; set; }

        public Mps000482PDO(
            List<Mps000482RDO> _mrs000482RDOs,
            List<Mps000482RDO> _ListBloods,
            SingKey482 _singKey482
            )
        {
            this._mrs000482RDOs = _mrs000482RDOs;
            this._ListBloods = _ListBloods;
            this._SingKey482 = _singKey482;
        }

    }

    public class SingKey482
    {
        public string TIME_TO_STR { get; set; }
        public string TIME_FROM_STR { get; set; }
        public string BLOOD_TYPE_CODE { get; set; }
        public string BLOOD_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string MEDI_STOCK_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string MEDI_BEGIN_AMOUNT { get; set; }
        public string MEDI_END_AMOUNT { get; set; }
        public Dictionary<string, object> DIC_OTHER_KEY { get; set; }
        //ADO
    }

    public class Mps000482RDO
    {
        public long? EXECUTE_TIME { get; set; }
        public string EXECUTE_DATE_STR { get; set; }
        public string IMP_MEST_CODE { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public long? EXPIRED_DATE { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public decimal BEGIN_AMOUNT { get; set; }
        public decimal? IMP_AMOUNT { get; set; }
        public decimal? EXP_AMOUNT { get; set; }
        public decimal END_AMOUNT { get; set; }

        public Mps000482RDO() { }

        public Mps000482RDO(List<V_HIS_EXP_MEST_BLOOD> exp)
        {
            try
            {
                EXECUTE_TIME = exp.First().EXP_TIME;
                PACKAGE_NUMBER = exp.First().PACKAGE_NUMBER;
                EXP_MEST_CODE = exp.First().EXP_MEST_CODE;
                EXPIRED_DATE = exp.First().EXPIRED_DATE;
                EXP_AMOUNT = exp.Count();

                SetExtendField(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000482RDO(List<V_HIS_IMP_MEST_BLOOD> imp)
        {
            try
            {
                EXECUTE_TIME = imp.First().IMP_TIME;
                PACKAGE_NUMBER = imp.First().PACKAGE_NUMBER;
                IMP_MEST_CODE = imp.First().IMP_MEST_CODE;
                EXPIRED_DATE = imp.First().EXPIRED_DATE;
                IMP_AMOUNT = imp.Count();
                SetExtendField(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetExtendField(Mps000482RDO data)
        {
            EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXECUTE_TIME ?? 0);
            EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
        }

        public void CalculateAmount(decimal previousEndAmount)
        {
            try
            {
                BEGIN_AMOUNT = previousEndAmount;
                END_AMOUNT = BEGIN_AMOUNT + (IMP_AMOUNT.HasValue ? IMP_AMOUNT.Value : 0) - (EXP_AMOUNT.HasValue ? EXP_AMOUNT.Value : 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
