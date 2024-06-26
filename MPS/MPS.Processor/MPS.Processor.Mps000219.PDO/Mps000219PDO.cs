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

namespace MPS.Processor.Mps000219.PDO
{
    public partial class Mps000219PDO : RDOBase
    {
        public List<Mrs00076RDO> _Mrs00076RDOs { get; set; }
        public List<Mrs00076RDO> _ListMaterials { get; set; }
        public SingKey219 _SingKey219 { get; set; }

        public Mps000219PDO(
            List<Mrs00076RDO> _mrs00076RDOs,
            List<Mrs00076RDO> _listMaterials,
            SingKey219 _singKey219
            )
        {
            this._Mrs00076RDOs = _mrs00076RDOs;
            this._ListMaterials = _listMaterials;
            this._SingKey219 = _singKey219;
        }
    }

    public class SingKey219
    {
        public string BEGIN_AMOUNT { get; set; }
        public string END_AMOUNT { get; set; }
        public string TIME_TO_STR { get; set; }
        public string TIME_FROM_STR { get; set; }
        public string MATERIAL_TYPE_CODE { get; set; }
        public string MATERIAL_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string MEDI_STOCK_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string MEDI_BEGIN_AMOUNT { get; set; }
        public string MEDI_END_AMOUNT { get; set; }
        //ADO
        public string CONCENTRA { get; set; }
        public string REGISTER_NUMBER { get; set; } // ADO chưa có
        public Dictionary<string, object> DIC_OTHER_KEY { get; set; }

    }

    public class Mrs00076RDO
    {
        public long? EXECUTE_TIME { get; set; }
        public string EXECUTE_DATE_STR { get; set; }
        public string IMP_MEST_CODE { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public string BID_NUMBER { get; set; }
        public long? EXPIRED_DATE { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public decimal BEGIN_AMOUNT { get; set; }
        public decimal? IMP_AMOUNT { get; set; }
        public decimal? EXP_AMOUNT { get; set; }
        public decimal END_AMOUNT { get; set; }
        public string PACKAGE_NUMBER { get; set; }

        public decimal? IMP_AMOUNT_KHONG_GOM_HOAN { get; set; }
        public decimal? IMP_AMOUNT_HOAN { get; set; }
        public decimal? CHMS_TYPE_ID { get; set; }

        public Mrs00076RDO() { }
        public Mrs00076RDO(V_HIS_IMP_MEST_MATERIAL imp)
        {
            try
            {
                EXECUTE_TIME = imp.IMP_TIME;
                BID_NUMBER = imp.BID_NUMBER;
                IMP_MEST_CODE = imp.IMP_MEST_CODE;
                EXPIRED_DATE = imp.EXPIRED_DATE;
                IMP_AMOUNT = imp.AMOUNT;
                PACKAGE_NUMBER = imp.PACKAGE_NUMBER;
                CHMS_TYPE_ID = imp.CHMS_TYPE_ID;
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

        public Mrs00076RDO(V_HIS_EXP_MEST_MATERIAL exp)
        {
            try
            {
                EXECUTE_TIME = exp.EXP_TIME;
                BID_NUMBER = exp.BID_NUMBER;
                EXP_MEST_CODE = exp.EXP_MEST_CODE;
                EXPIRED_DATE = exp.EXPIRED_DATE;
                EXP_AMOUNT = exp.AMOUNT;
                PACKAGE_NUMBER = exp.PACKAGE_NUMBER;
                SetExtendField(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetExtendField(Mrs00076RDO data)
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
