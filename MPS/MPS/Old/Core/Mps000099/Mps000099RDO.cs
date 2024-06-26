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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000099
{
    public class Mps000099RDO : RDOBase
    {
        V_HIS_SALE_EXP_MEST _SaleExpMest = null;
        List<V_HIS_EXP_MEST_MEDICINE> _Medicines = null;
        string GenderName = null;
        long? DOB = null;

        long expMesttSttId__Draft = 1;// trạng thái nháp
        long expMesttSttId__Request = 2;// trạng thái yêu cầu
        long expMesttSttId__Reject = 3;// không duyệt
        long expMesttSttId__Approval = 4; // duyệt
        long expMesttSttId__Export = 5;// đã xuất

        public Mps000099RDO(V_HIS_SALE_EXP_MEST saleExpMest, List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines, string genderName, long? dob, long _expMesttSttId__Draft, long _expMesttSttId__Request, long _expMesttSttId__Reject, long _expMesttSttId__Approval, long _expMesttSttId__Export)
        {
            this._SaleExpMest = saleExpMest;
            this._Medicines = expMestMedicines;
            this.GenderName = genderName;
            this.DOB = dob;
            this.expMesttSttId__Draft = _expMesttSttId__Draft;
            this.expMesttSttId__Request = _expMesttSttId__Request;
            this.expMesttSttId__Reject = _expMesttSttId__Reject;
            this.expMesttSttId__Approval = _expMesttSttId__Approval;
            this.expMesttSttId__Export = _expMesttSttId__Export;
        }
     public Mps000099RDO(V_HIS_SALE_EXP_MEST saleExpMest, List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines, string genderName, long? dob)
        {
            this._SaleExpMest = saleExpMest;
            this._Medicines = expMestMedicines;
            this.GenderName = genderName;
            this.DOB = dob;
        }
        internal override void SetSingleKey()
        {
            try
            {
                if (this._SaleExpMest != null)
                {
                    if (this.DOB.HasValue && this.DOB.Value > 0)
                    {
                        keyValues.Add(new KeyValue(Mps000099ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.Age(this.DOB ?? 0)));
                    }
                    else
                    {
                        keyValues.Add(new KeyValue(Mps000099ExtendSingleKey.AGE, null));
                    }
                    keyValues.Add(new KeyValue(Mps000099ExtendSingleKey.GENDER_NAME, this.GenderName));
                    if (this._SaleExpMest.PATIENT_ID.HasValue)
                    {
                        keyValues.Add(new KeyValue(Mps000099ExtendSingleKey.PATIENT_CODE, this._SaleExpMest.PATIENT_CODE));
                        keyValues.Add(new KeyValue(Mps000099ExtendSingleKey.PATIENT_NAME, this._SaleExpMest.VIR_PATIENT_NAME));
                    }
                    else
                    {
                        keyValues.Add(new KeyValue(Mps000099ExtendSingleKey.PATIENT_CODE, ""));
                        keyValues.Add(new KeyValue(Mps000099ExtendSingleKey.PATIENT_NAME, this._SaleExpMest.CLIENT_NAME));
                    }
                    keyValues.Add(new KeyValue(Mps000099ExtendSingleKey.REQ_USERNAME, this._SaleExpMest.REQ_USERNAME));
                    keyValues.Add(new KeyValue(Mps000099ExtendSingleKey.EXP_USERNAME, this._SaleExpMest.EXP_USERNAME));
                }

                if (this._Medicines != null && this._Medicines.Count > 0)
                {
                    // sắp xếp theo thứ tự tăng dần của id
                    this._Medicines = this._Medicines.OrderBy(o => o.ID).ToList();
                    if (_SaleExpMest.EXP_MEST_STT_ID == expMesttSttId__Approval || _SaleExpMest.EXP_MEST_STT_ID == expMesttSttId__Export)
                    {
                        this._Medicines = this._Medicines.Where(o => o.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                    }
                    else if (_SaleExpMest.EXP_MEST_STT_ID == expMesttSttId__Request || _SaleExpMest.EXP_MEST_STT_ID == expMesttSttId__Reject || _SaleExpMest.EXP_MEST_STT_ID == expMesttSttId__Draft)
                    {
                        this._Medicines = this._Medicines.Where(o => o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                    }
                    keyValues.Add(new KeyValue(Mps000099ExtendSingleKey.MEDICINE_TYPE_NAME, this._Medicines.First().MEDICINE_TYPE_NAME));
                    keyValues.Add(new KeyValue(Mps000099ExtendSingleKey.AMOUNT, this._Medicines.Sum(o => o.AMOUNT)));
                    keyValues.Add(new KeyValue(Mps000099ExtendSingleKey.SERVICE_UNIT_NAME, this._Medicines.First().SERVICE_UNIT_NAME));
                    keyValues.Add(new KeyValue(Mps000099ExtendSingleKey.TUTORIAL, this._Medicines.First().TUTORIAL));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
