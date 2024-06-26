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
using MOS.SDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000218.PDO
{
    public partial class Mps000218PDO : RDOBase
    {
        public List<Mrs00075RDO> _Mrs00075RDOs { get; set; }
        public SingKey218 _SingKey218 { get; set; }
        public List<Mrs00075RDO> _ListMedicines { get; set; }

        public Mps000218PDO(
            List<Mrs00075RDO> _mrs00075RDOs,
            List<Mrs00075RDO> _listMedicines,
            SingKey218 _singKey218
            )
        {
            this._Mrs00075RDOs = _mrs00075RDOs;
            this._ListMedicines = _listMedicines;
            this._SingKey218 = _singKey218;
        }
    }

    public class SingKey218
    {
        public string TIME_TO_STR { get; set; }
        public string TIME_FROM_STR { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string MEDI_STOCK_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string MEDI_BEGIN_AMOUNT { get; set; }
        public string MEDI_END_AMOUNT { get; set; }
        //ADO
        public string CONCENTRA { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public Dictionary<string,object> DIC_OTHER_KEY { get; set; }
    }

    public class Mrs00075RDO
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

        public decimal? IMP_AMOUNT_KHONG_GOM_HOAN { get; set; }
        public decimal? IMP_AMOUNT_HOAN { get; set; }
        public decimal? CHMS_TYPE_ID { get; set; }
        public string DOCUMENT_NUMBER { get; set; }
        public Mrs00075RDO() { }

        public Mrs00075RDO(V_HIS_IMP_MEST_MEDICINE imp)
        {
            try
            {
                EXECUTE_TIME = imp.IMP_TIME;
                PACKAGE_NUMBER = imp.PACKAGE_NUMBER;
                IMP_MEST_CODE = imp.IMP_MEST_CODE;
                EXPIRED_DATE = imp.EXPIRED_DATE;
                IMP_AMOUNT = imp.AMOUNT;
                CHMS_TYPE_ID = imp.CHMS_TYPE_ID;
                DOCUMENT_NUMBER = imp.DOCUMENT_NUMBER;
                if (imp.CHMS_TYPE_ID == 2)
                {
                    IMP_AMOUNT_HOAN = imp.AMOUNT;
                }
                else
                {
                    IMP_AMOUNT_KHONG_GOM_HOAN = imp.AMOUNT;
                }
                SetExtendField(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mrs00075RDO(V_HIS_EXP_MEST_MEDICINE exp)
        {
            try
            {
                EXECUTE_TIME = exp.EXP_TIME;
                PACKAGE_NUMBER = exp.PACKAGE_NUMBER;
                EXP_MEST_CODE = exp.EXP_MEST_CODE;
                EXPIRED_DATE = exp.EXPIRED_DATE;
                EXP_AMOUNT = exp.AMOUNT;

                SetExtendField(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetExtendField(Mrs00075RDO data)
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
