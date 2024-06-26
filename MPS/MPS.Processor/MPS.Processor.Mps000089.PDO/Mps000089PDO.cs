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

namespace MPS.Processor.Mps000089.PDO
{
    public class Mps000089PDO : RDOBase
    {
        public V_HIS_EXP_MEST _ChmsExpMest = null;
        public List<V_HIS_EXP_MEST_MEDICINE> _Medicines = null;

        public long expMesttSttId__Draft = 1;// trạng thái nháp
        public long expMesttSttId__Request = 2;// trạng thái yêu cầu
        public long expMesttSttId__Reject = 3;// không duyệt
        public long expMesttSttId__Approval = 4; // duyệt
        public long expMesttSttId__Export = 5;// đã xuất

        public List<Mps000089ADO> listAdo = new List<Mps000089ADO>();

        public Mps000089PDO() { }

        public Mps000089PDO(V_HIS_EXP_MEST chmsExpMest, List<V_HIS_EXP_MEST_MEDICINE> listMedicine)
        {
            this._ChmsExpMest = chmsExpMest;
            this._Medicines = listMedicine;
        }

        public Mps000089PDO(
            V_HIS_EXP_MEST chmsExpMest,
            List<V_HIS_EXP_MEST_MEDICINE> listMedicine,
            long _expMesttSttId__Draft,
            long _expMesttSttId__Request,
            long _expMesttSttId__Reject,
            long _expMesttSttId__Approval,
            long _expMesttSttId__Export)
        {
            this._ChmsExpMest = chmsExpMest;
            this._Medicines = listMedicine;
            this.expMesttSttId__Draft = _expMesttSttId__Draft;
            this.expMesttSttId__Request = _expMesttSttId__Request;
            this.expMesttSttId__Reject = _expMesttSttId__Reject;
            this.expMesttSttId__Approval = _expMesttSttId__Approval;
            this.expMesttSttId__Export = _expMesttSttId__Export;
        }
    }
    public class Mps000089ADO : V_HIS_EXP_MEST_MEDICINE
    {
        public string EXPIRED_DATE_STR { get; set; }

        public Mps000089ADO()
        { }

        public Mps000089ADO(List<V_HIS_EXP_MEST_MEDICINE> listMedicine)
        {
            try
            {
                if (listMedicine != null && listMedicine.Count > 0)
                {
                    this.MEDICINE_TYPE_ID = listMedicine.First().MEDICINE_TYPE_ID;
                    this.MEDICINE_TYPE_CODE = listMedicine.First().MEDICINE_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = listMedicine.First().MEDICINE_TYPE_NAME;
                    this.SERVICE_UNIT_CODE = listMedicine.First().SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = listMedicine.First().SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = listMedicine.First().SUPPLIER_CODE;
                    this.SUPPLIER_NAME = listMedicine.First().SUPPLIER_NAME;
                    this.NUM_ORDER = listMedicine.First().NUM_ORDER;
                    this.PACKAGE_NUMBER = listMedicine.First().PACKAGE_NUMBER;
                    this.REGISTER_NUMBER = listMedicine.First().REGISTER_NUMBER;
                    this.BID_NAME = listMedicine.First().BID_NAME;
                    this.BID_NUMBER = listMedicine.First().BID_NUMBER;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMedicine.First().EXPIRED_DATE ?? 0);
                    this.AMOUNT = listMedicine.Sum(s => s.AMOUNT);
                    this.MEDICINE_NUM_ORDER = listMedicine.First().MEDICINE_NUM_ORDER;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
