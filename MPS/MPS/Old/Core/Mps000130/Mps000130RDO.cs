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
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000130
{
    /// <summary>
    /// In yeu cau kham.
    /// Dau vao:
    /// PatyAlterBhyt: doi tuong the bhyt
    /// ServiceReq: yeu cau dich vu
    /// PatientType: doi tuong benh nhan
    /// </summary>
    /// 
    public class Mps000130RDO : RDOBase
    {
        internal HIS_EXP_MEST expMest { get; set; }
        internal V_HIS_MANU_IMP_MEST manuImpMest { get; set; }
        internal List<Mps000130ADO> listMrs000084ADO;

        List<V_HIS_EXP_MEST_MEDICINE> _Medicines = null;
        List<V_HIS_EXP_MEST_MATERIAL> _Materials = null;

        long expMesttSttId__Draft = 1;// trạng thái nháp
        long expMesttSttId__Request = 2;// trạng thái yêu cầu
        long expMesttSttId__Reject = 3;// không duyệt
        long expMesttSttId__Approval = 4; // duyệt
        long expMesttSttId__Export = 5;// đã xuất

        public Mps000130RDO(
            HIS_EXP_MEST expMest,
            V_HIS_MANU_IMP_MEST manuImpMest,
            List<V_HIS_EXP_MEST_MEDICINE> hisImpMestMedicines,
            List<V_HIS_EXP_MEST_MATERIAL> hisImpMestMaterials
            )
        {
            try
            {
                this.expMest = expMest;
                this.manuImpMest = manuImpMest;
                this._Medicines = hisImpMestMedicines;
                this._Materials = hisImpMestMaterials;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        } 
        
        public Mps000130RDO(
            HIS_EXP_MEST expMest,
            V_HIS_MANU_IMP_MEST manuImpMest,
            List<V_HIS_EXP_MEST_MEDICINE> hisImpMestMedicines,
            List<V_HIS_EXP_MEST_MATERIAL> hisImpMestMaterials, long _expMesttSttId__Draft, long _expMesttSttId__Request, long _expMesttSttId__Reject, long _expMesttSttId__Approval, long _expMesttSttId__Export
            )
        {
            try
            {
                this.expMest = expMest;
                this.manuImpMest = manuImpMest;
                this._Medicines = hisImpMestMedicines;
                this._Materials = hisImpMestMaterials;
                this.expMesttSttId__Draft = _expMesttSttId__Draft;
                this.expMesttSttId__Request = _expMesttSttId__Request;
                this.expMesttSttId__Reject = _expMesttSttId__Reject;
                this.expMesttSttId__Approval = _expMesttSttId__Approval;
                this.expMesttSttId__Export = _expMesttSttId__Export;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                GlobalQuery.AddObjectKeyIntoListkey<HIS_EXP_MEST>(expMest, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_MANU_IMP_MEST>(manuImpMest, keyValues, false);

                keyValues.Add(new KeyValue(Mps000130ExtendSingleKey.EXP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((expMest.EXP_TIME ?? 0))));

                listMrs000084ADO = new List<Mps000130ADO>();
                if (_Medicines != null && _Medicines.Count > 0)
                {
                    // sắp xếp theo thứ tự tăng dần id
                    this._Medicines = this._Medicines.OrderBy(p => p.ID).ToList();
                    if (expMest.EXP_MEST_STT_ID == expMesttSttId__Approval || expMest.EXP_MEST_STT_ID == expMesttSttId__Export)
                    {
                        this._Medicines = this._Medicines.Where(o => o.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                    }
                    else if (expMest.EXP_MEST_STT_ID == expMesttSttId__Request || expMest.EXP_MEST_STT_ID == expMesttSttId__Reject || expMest.EXP_MEST_STT_ID == expMesttSttId__Draft)
                    {
                        this._Medicines = this._Medicines.Where(o => o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                    }
                    listMrs000084ADO.AddRange((from r in _Medicines select new Mps000130ADO(r)).ToList());
                }
                if (_Materials != null && _Materials.Count > 0)
                {
                    // sắp xếp theo thứ tự tăng dần id
                    this._Materials = this._Materials.OrderBy(p => p.ID).ToList();
                    if (expMest.EXP_MEST_STT_ID == expMesttSttId__Approval || expMest.EXP_MEST_STT_ID == expMesttSttId__Export)
                    {
                        this._Materials = this._Materials.Where(o => o.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MATERIAL.IN_EXECUTE__TRUE).ToList();
                    }
                    else if (expMest.EXP_MEST_STT_ID == expMesttSttId__Request || expMest.EXP_MEST_STT_ID == expMesttSttId__Reject || expMest.EXP_MEST_STT_ID == expMesttSttId__Draft)
                    {
                        this._Materials = this._Materials.Where(o => o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MATERIAL.IN_REQUEST__TRUE).ToList();
                    }
                    listMrs000084ADO.AddRange((from r in _Materials select new Mps000130ADO(r)).ToList());
                }

                if (listMrs000084ADO != null)
                {
                    decimal sumPrice = listMrs000084ADO.Sum(p => p.TOTAL_PRICE);
                    keyValues.Add(new KeyValue(Mps000130ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(sumPrice).ToString())));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
