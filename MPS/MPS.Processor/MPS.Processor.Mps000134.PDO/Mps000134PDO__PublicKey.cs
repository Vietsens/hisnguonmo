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
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000134.PDO
{
    public partial class Mps000134PDO : RDOBase
    {
        public List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReqs = null;
        public List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReqs = null;
        public V_HIS_EXP_MEST _ExpMest = null;
        public long expMesttSttId__Draft = 1;// trạng thái nháp
        public long expMesttSttId__Request = 2;// trạng thái yêu cầu
        public long expMesttSttId__Reject = 3;// không duyệt
        public long expMesttSttId__Approval = 4; // duyệt
        public long expMesttSttId__Export = 5;// đã xuất
        public string Title = "";
        public List<Mps000134ADO> listAdo = new List<Mps000134ADO>();
    }

    public class Mps000134ADO
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
        public string DESCRIPTION { get; set; }

        public long? NUM_ORDER { get; set; }

        public Mps000134ADO()
        {
        }

        public Mps000134ADO(List<V_HIS_EXP_MEST_MEDICINE> listMedicine)
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
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000134ADO(List<V_HIS_EXP_MEST_MATERIAL> listMaterial)
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
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000134ADO(List<HIS_EXP_MEST_METY_REQ> _expMestMetyReqs, List<V_HIS_MEDICINE_TYPE> _medicineTypes)
        {
            try
            {
                if (_expMestMetyReqs != null && _expMestMetyReqs.Count > 0)
                {
                    this.TYPE_ID = 1;
                    var data = _medicineTypes.FirstOrDefault(p => p.ID == _expMestMetyReqs.First().MEDICINE_TYPE_ID);
                    if (data != null)
                    {
                        this.MEDI_MATE_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                        this.MEDI_MATE_TYPE_ID = data.ID;
                        this.MEDI_MATE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                        this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                        this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                        this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    }
                    this.AMOUNT = _expMestMetyReqs.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = _expMestMetyReqs.First().NUM_ORDER;
                    this.DESCRIPTION = _expMestMetyReqs.First().DESCRIPTION;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000134ADO(List<HIS_EXP_MEST_MATY_REQ> _expMestMatyReqs, List<V_HIS_MATERIAL_TYPE> _materialTypes)
        {
            try
            {
                if (_expMestMatyReqs != null && _expMestMatyReqs.Count > 0)
                {
                    this.TYPE_ID = 2;
                    var data = _materialTypes.FirstOrDefault(p => p.ID == _expMestMatyReqs.First().MATERIAL_TYPE_ID);
                    if (data != null)
                    {
                        this.MEDI_MATE_TYPE_CODE = data.MATERIAL_TYPE_CODE;
                        this.MEDI_MATE_TYPE_ID = data.ID;
                        this.MEDI_MATE_TYPE_NAME = data.MATERIAL_TYPE_NAME;
                        //this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                        this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                        this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    }
                    this.AMOUNT = _expMestMatyReqs.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = _expMestMatyReqs.First().NUM_ORDER;
                    this.DESCRIPTION = _expMestMatyReqs.First().DESCRIPTION;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
