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

namespace MPS.Processor.Mps000198.PDO
{
    public partial class Mps000198PDO : RDOBase
    {
        public List<HIS_EXP_MEST_BLTY_REQ> _ExpMestBltyReqs = null;
        public List<V_HIS_EXP_MEST_BLOOD> _ExpMestBloods = null;
        public List<V_HIS_MEDI_STOCK> _MediStocks = null;
        public List<V_HIS_BLOOD_TYPE> _BloodTypes = null;
        public List<HIS_BLOOD_ABO> _BloodABOs = null;
        public List<HIS_BLOOD_RH> _BloodRHs = null;
        public long expMesttSttId__Approval = 4; // duyệt
        public long expMesttSttId__Export = 5;// đã xuất
        public long _keyMert = 1;
        public long _keyPhieuTra = 0;
        public string KeyNames = "";

        public List<Mps000198ADO> listAdo = new List<Mps000198ADO>();

        public Mps000198PDO() { }

        public Mps000198PDO(
            V_HIS_EXP_MEST chmsExpMest,
            List<HIS_EXP_MEST_BLTY_REQ> _expMestBltys,
            List<V_HIS_EXP_MEST_BLOOD> _expMestBloods,
            List<V_HIS_MEDI_STOCK> _mediStocks,
            List<V_HIS_BLOOD_TYPE> _bloodTypes,
            List<HIS_BLOOD_ABO> _bloodABOs,
            List<HIS_BLOOD_RH> _bloodRHs,
            long _expMesttSttId__Approval,
            long _expMesttSttId__Export,
            string tittle,
            long keyMert,
            long keyPhieuTra
            )
        {
            this._ChmsExpMest = chmsExpMest;
            this._ExpMestBltyReqs = _expMestBltys;
            this._ExpMestBloods = _expMestBloods;
            this._MediStocks = _mediStocks;
            this._BloodTypes = _bloodTypes;
            this._BloodABOs = _bloodABOs;
            this._BloodRHs = _bloodRHs;
            this.expMesttSttId__Approval = _expMesttSttId__Approval;
            this.expMesttSttId__Export = _expMesttSttId__Export;
            this.KeyNames = tittle;
            this._keyMert = keyMert;
            this._keyPhieuTra = keyPhieuTra;
        }
    }
}
