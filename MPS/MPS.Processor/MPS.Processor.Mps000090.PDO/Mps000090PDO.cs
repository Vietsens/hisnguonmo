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

namespace MPS.Processor.Mps000090.PDO
{
    public class Mps000090PDO : RDOBase
    {
        public V_HIS_EXP_MEST _ChmsExpMest;
        public List<V_HIS_EXP_MEST_MEDICINE> _Medicines = null;
        public List<V_HIS_EXP_MEST_MATERIAL> _ListMaterial = null;

        public long expMesttSttId__Draft = 1;// trạng thái nháp
        public long expMesttSttId__Request = 2;// trạng thái yêu cầu
        public long expMesttSttId__Reject = 3;// không duyệt
        public long expMesttSttId__Approval = 4; // duyệt
        public long expMesttSttId__Export = 5;// đã xuất

        public List<Mps000090ADO> listAdo = new List<Mps000090ADO>();

        public Mps000090PDO(V_HIS_EXP_MEST chmsExpMest, List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial)
        {
            this._ChmsExpMest = chmsExpMest;
            this._ListMaterial = listMaterial;
            this._Medicines = listMedicine;
        }
        public Mps000090PDO(V_HIS_EXP_MEST chmsExpMest, List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial, long _expMesttSttId__Draft, long _expMesttSttId__Request, long _expMesttSttId__Reject, long _expMesttSttId__Approval, long _expMesttSttId__Export)
        {
            this._ChmsExpMest = chmsExpMest;
            this._ListMaterial = listMaterial;
            this._Medicines = listMedicine;
            this.expMesttSttId__Draft = _expMesttSttId__Draft;
            this.expMesttSttId__Request = _expMesttSttId__Request;
            this.expMesttSttId__Reject = _expMesttSttId__Reject;
            this.expMesttSttId__Approval = _expMesttSttId__Approval;
            this.expMesttSttId__Export = _expMesttSttId__Export;
        }
    }
    public class Mps000090ADO
    {
        public long TYPE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }

        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string BID_NUMBER { get; set; }
        public string BID_NAME { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public decimal AMOUNT { get; set; }
        public long MEDI_MATE_NUM_ORDER { get; set; }

        public long? NUM_ORDER { get; set; }

        public Mps000090ADO()
        {
        }

        public Mps000090ADO(List<V_HIS_EXP_MEST_MEDICINE> listMedicine)
        {
            try
            {
                if (listMedicine != null && listMedicine.Count > 0)
                {
                    this.TYPE_ID = 1;
                    this.MEDI_MATE_TYPE_CODE = listMedicine.First().MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = listMedicine.First().MEDICINE_TYPE_ID;
                    this.MEDI_MATE_TYPE_NAME = listMedicine.First().MEDICINE_TYPE_NAME;
                    this.PACKAGE_NUMBER = listMedicine.First().PACKAGE_NUMBER;
                    this.REGISTER_NUMBER = listMedicine.First().REGISTER_NUMBER;
                    this.SERVICE_UNIT_CODE = listMedicine.First().SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = listMedicine.First().SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = listMedicine.First().SUPPLIER_CODE;
                    this.SUPPLIER_NAME = listMedicine.First().SUPPLIER_NAME;
                    this.BID_NAME = listMedicine.First().BID_NAME;
                    this.BID_NUMBER = listMedicine.First().BID_NUMBER;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMedicine.First().EXPIRED_DATE ?? 0);
                    this.AMOUNT = listMedicine.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = listMedicine.First().NUM_ORDER;
                    this.MEDI_MATE_NUM_ORDER = listMedicine.First().MEDICINE_NUM_ORDER ?? 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000090ADO(List<V_HIS_EXP_MEST_MATERIAL> listMaterial)
        {
            try
            {
                if (listMaterial != null && listMaterial.Count > 0)
                {
                    this.TYPE_ID = 2;
                    this.MEDI_MATE_TYPE_CODE = listMaterial.First().MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = listMaterial.First().MATERIAL_TYPE_ID;
                    this.MEDI_MATE_TYPE_NAME = listMaterial.First().MATERIAL_TYPE_NAME;
                    this.PACKAGE_NUMBER = listMaterial.First().PACKAGE_NUMBER;
                    this.SERVICE_UNIT_CODE = listMaterial.First().SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = listMaterial.First().SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = listMaterial.First().SUPPLIER_CODE;
                    this.SUPPLIER_NAME = listMaterial.First().SUPPLIER_NAME;
                    this.BID_NAME = listMaterial.First().BID_NAME;
                    this.BID_NUMBER = listMaterial.First().BID_NUMBER;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMaterial.First().EXPIRED_DATE ?? 0);
                    this.AMOUNT = listMaterial.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = listMaterial.First().NUM_ORDER;
                    this.MEDI_MATE_NUM_ORDER = listMaterial.First().MATERIAL_NUM_ORDER ?? 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
